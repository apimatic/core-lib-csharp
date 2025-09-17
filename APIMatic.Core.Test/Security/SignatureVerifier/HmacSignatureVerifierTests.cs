using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using APIMatic.Core.Security.SignatureVerifier;
using APIMatic.Core.Test.MockTypes.Http.Request;
using APIMatic.Core.Types;
using NUnit.Framework;

namespace APIMatic.Core.Test.Security.SignatureVerifier;

[TestFixture]
public class HmacSignatureVerifierTests
{
    private const string SecretKey = "test_secret";
    private const string HeaderName = "X-Signature";
    private const string Payload = "hello world";

    private static HttpRequestData CreateRequest(string headerValue, string headerName = HeaderName, string payload = Payload)
    {
        var headers = headerValue == null 
            ? new Dictionary<string, string[]>()
            : new Dictionary<string, string[]> { { headerName, new[] { headerValue } } };
        return new HttpRequestData(headers, new MemoryStream(System.Text.Encoding.UTF8.GetBytes(payload)));
    }

    private static HmacSignatureVerifier CreateVerifier(
        EncodingType encodingType,
        string headerName = HeaderName,
        string secretKey = SecretKey,
        string signatureValueTemplate = "{digest}")
    {
        return new HmacSignatureVerifier(secretKey, headerName, encodingType, signatureValueTemplate: signatureValueTemplate);
    }

    [Test]
    public void Constructor_ThrowsOnNullOrEmptySecretKey_OnCreate_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => CreateVerifier(EncodingType.Hex, secretKey: null));
        Assert.Throws<ArgumentNullException>(() => CreateVerifier(EncodingType.Hex, secretKey: ""));
    }

    [Test]
    public void Constructor_ThrowsOnNullOrEmptyHeader_OnCreate_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => CreateVerifier(EncodingType.Hex, headerName: null));
        Assert.Throws<ArgumentNullException>(() => CreateVerifier(EncodingType.Hex, headerName: ""));
    }

    [Test]
    public async Task NullHeader_OnVerifyAsync_ReturnsFailure()
    {
        var request = CreateRequest(null);
        var verifier = CreateVerifier(EncodingType.Hex);
        var result = await verifier.VerifyAsync(request);
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual($"Signature header '{HeaderName}' is missing.", result.Errors.First());
    }
    
    [Test]
    public async Task MissingHeader_OnVerifyAsync_ReturnsFailure()
    {
        var request = CreateRequest(string.Empty);
        var verifier = CreateVerifier(EncodingType.Hex);
        var result = await verifier.VerifyAsync(request);
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual($"Malformed signature header '{HeaderName}' value.", result.Errors.First());
    }

    [Test]
    public async Task MalformedHeader_OnVerifyAsync_ReturnsFailure()
    {
        var request = CreateRequest("");
        var verifier = CreateVerifier(EncodingType.Hex);
        var result = await verifier.VerifyAsync(request);
        Assert.IsFalse(result.IsSuccess);
        StringAssert.Contains("Malformed", result.Errors.First());
    }

    [Test]
    public async Task SignatureDecodingFails_OnVerifyAsync_ReturnsFailure()
    {
        var request = CreateRequest("not-a-valid-hex");
        var verifier = CreateVerifier(EncodingType.Hex);
        var result = await verifier.VerifyAsync(request);
        Assert.IsFalse(result.IsSuccess);
    }

    [TestCase(EncodingType.Hex)]
    [TestCase(EncodingType.Base64)]
    [TestCase(EncodingType.Base64Url)]
    public async Task CorrectSignature_OnVerifyAsync_ReturnsSuccess(EncodingType encodingType)
    {
        string encodedDigest = GetDigest(encodingType, SecretKey, Payload);
        var request = CreateRequest(encodedDigest);
        var verifier = CreateVerifier(encodingType);
        var result = await verifier.VerifyAsync(request);
        Assert.IsTrue(result.IsSuccess);
    }

    [TestCase(EncodingType.Hex, "deadbeef")]
    [TestCase(EncodingType.Base64, "Zm9vYmFyYmF6")]
    [TestCase(EncodingType.Base64Url, "Zm9vYmFyYmF6")]
    public async Task IncorrectSignature_OnVerifyAsync_ReturnsFailure(EncodingType encodingType, string badDigest)
    {
        var request = CreateRequest(badDigest);
        var verifier = CreateVerifier(encodingType);
        var result = await verifier.VerifyAsync(request);
        Assert.IsFalse(result.IsSuccess);
        StringAssert.Contains("failed", result.Errors.First());
    }

    [Test]
    public async Task TemplateExtractsDigest_OnVerifyAsync_ReturnsSuccess()
    {
        var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(SecretKey));
        var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Payload));
        var digest = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        const string template = "prefix-{digest}-suffix";
        var signatureValue = $"prefix-{digest}-suffix";
        var request = CreateRequest(signatureValue);
        var verifier = CreateVerifier(EncodingType.Hex, signatureValueTemplate: template);
        var result = await verifier.VerifyAsync(request);
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task TemplateDoesNotMatch_OnVerifyAsync_ReturnsFailure()
    {
        const string template = "prefix-{digest}-suffix";
        const string signatureValue = $"wrongprefix-deadbeef-wrongsuffix";
        var request = CreateRequest(signatureValue);
        var verifier = CreateVerifier(EncodingType.Hex, signatureValueTemplate: template);
        var result = await verifier.VerifyAsync(request);
        Assert.IsFalse(result.IsSuccess);
    } 
    
    [Test]
    public async Task TemplateDoesNotContainDigest_OnVerifyAsync_ReturnsFailure()
    {
        const string template = "prefix-{wrong}-suffix";
        const string signatureValue = $"wrongprefix-deadbeef-wrongsuffix";
        var request = CreateRequest(signatureValue);
        var verifier = CreateVerifier(EncodingType.Hex, signatureValueTemplate: template);
        var result = await verifier.VerifyAsync(request);
        Assert.IsFalse(result.IsSuccess);
    }
    
    [Test]
    public async Task CorrectDigestButIncorrectExpectedTemplate_OnVerifyAsync_ReturnsFailure()
    {
        var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(SecretKey));
        var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Payload));
        var digest = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        // The expected template doesn't match the signature value template, though digest is correct
        const string expectedTemplate = "sha26={digest}";
        var signatureValue = $"sha256={digest}";
        var request = CreateRequest(signatureValue);
        var verifier = CreateVerifier(EncodingType.Hex, signatureValueTemplate: expectedTemplate);
        var result = await verifier.VerifyAsync(request);
        Assert.IsFalse(result.IsSuccess);
    }
    
    [Test]
    public async Task CorrectDigestButIncorrectSignatureValue_OnVerifyAsync_ReturnsFailure()
    {
        var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(SecretKey));
        var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Payload));
        var digest = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        const string expectedTemplate = "sha256={digest}";
        // The signature value does not match the template, though digest is correct
        var signatureValue = $"sha25={digest}";
        var request = CreateRequest(signatureValue);
        var verifier = CreateVerifier(EncodingType.Hex, signatureValueTemplate: expectedTemplate);
        var result = await verifier.VerifyAsync(request);
        Assert.IsFalse(result.IsSuccess);
    }

    private static string GetDigest(EncodingType encodingType, string secretKey, string payload)
    {
        var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secretKey));
        var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payload));
        return encodingType switch
        {
            EncodingType.Hex => BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant(),
            EncodingType.Base64 => Convert.ToBase64String(hash),
            EncodingType.Base64Url => Convert.ToBase64String(hash).Replace('+', '-').Replace('/', '_').TrimEnd('='),
            _ => BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant()
        };
    }
}
