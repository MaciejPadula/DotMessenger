using DotMessenger.AzureEventHub.Infrastructure;
using DotMessenger.Contract;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework.Internal;
using System.Text.Json;

namespace DotMessenger.AzureEventHub.Tests;

public class JsonEventHubDataSerializerTests
{
    private JsonEventHubDataSerializer _sut;
    private ILogger<JsonEventHubDataSerializer> _logger;

    [SetUp]
    public void Setup()
    {
        _logger = Substitute.For<ILogger<JsonEventHubDataSerializer>>();
        _sut = new JsonEventHubDataSerializer(_logger);
    }

    [Test]
    public void Deserialize_WhenCalled_DeserializesMessage()
    {
        // Arrange
        var data = new byte[] { 0x7B, 0x22, 0x49, 0x64, 0x22, 0x3A, 0x31, 0x2C, 0x22, 0x4E, 0x61, 0x6D, 0x65, 0x22, 0x3A, 0x22, 0x54, 0x65, 0x73, 0x74, 0x22, 0x7D };
        var expectedResult = new TestMessage { Id = 1, Name = "Test" };

        // Act
        var result = _sut.Deserialize<TestMessage>(data);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Test]
    public void Deserialize_WhenDataIsNotJson_ShouldReturnNull()
    {
        // Arrange
        var data = "{\"I"u8.ToArray();

        // Act
        var result = _sut.Deserialize<TestMessage>(data);

        // Assert
        result.Should().BeNull();
        _logger.Received(1).LogError<JsonEventHubDataSerializer, JsonException>();
    }

    [Test]
    public void Serialize_WhenCalled_SerializesMessage()
    {
        // Arrange
        var message = new TestMessage { Id = 1, Name = "Test" };
        var expectedResult = "{\"Id\":1,\"Name\":\"Test\"}"u8.ToArray();

        // Act
        var result = _sut.Serialize(message);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Test]
    public void Message_WhenSerializedAndDeserialized_ShouldBeTheSame()
    {
        // Arrange
        var message = new TestMessage { Id = 1, Name = "Test" };

        // Act
        var serialized = _sut.Serialize(message);
        var deserialized = _sut.Deserialize<TestMessage>(serialized);

        // Assert
        deserialized.Should().BeEquivalentTo(message);
    }

    private class TestMessage : IMessage
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
    }
}