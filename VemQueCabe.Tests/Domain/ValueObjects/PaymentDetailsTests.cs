using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Tests.Domain.ValueObjects;

public class PaymentDetailsTests
{
    [Theory]
    [InlineData("4242424242424242", "Visa")] // Visa
    [InlineData("4012888888881881", "Visa")] // Visa
    [InlineData("5555555555554444", "MasterCard")] // MasterCard
    [InlineData("5105105105105100", "MasterCard")] // MasterCard
    [InlineData("378282246310005", "Amex")] // American Express
    [InlineData("371449635398431", "Amex")] // American Express
    [InlineData("6011111111111117", "Unknown")] // Unknown
    [InlineData("6011000990139424", "Unknown")] // Unknown
    [InlineData("5357939247127", "MasterCard")] // MasterCard
    [InlineData("5247243530505125661", "MasterCard")] // MasterCard
    public void CreatePaymentDetails_ValidData_ShouldCreatePaymentDetails(string cardNumber, string expectedCardHolderName)
    {
        // Arrange
        var expirationDate = DateOnly.FromDateTime(DateTime.Today.AddYears(7));
        var cvv = "123";
        
        // Act
        var paymentDetails = new PaymentDetails(cardNumber, expirationDate, cvv);
        
        // Assert
        Assert.NotNull(paymentDetails);
        Assert.Equal(cardNumber, paymentDetails.CardNumber);
        Assert.Equal(expirationDate, paymentDetails.ExpirationDate);
        Assert.Equal(cvv, paymentDetails.Cvv);
        Assert.Equal(expectedCardHolderName, paymentDetails.CardHolderName);
    }

    [Theory]
    [InlineData("1234-5678-9012")] 
    [InlineData("1234abcd5678efgh")] 
    [InlineData("4242-4242-4242-4242")] 
    [InlineData("4242 4242 4242 4242")] 
    public void CreatePaymentDetails_InvalidCardNumber_ShouldThrowArgumentException(string cardNumber)
    {
        // Arrange
        var expirationDate = DateOnly.FromDateTime(DateTime.Today.AddYears(7));
        var cvv = "123";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new PaymentDetails(cardNumber, expirationDate, cvv));
        
        // Assert
        Assert.Equal("cardNumber", exception.ParamName);
        Assert.Contains("Card number must contain only digits.", exception.Message);
    }

    [Fact]
    public void CreatePaymentDetails_NullCardNumber_ShouldThrowArgumentException()
    {
        // Arrange
        var expirationDate = DateOnly.FromDateTime(DateTime.Today.AddYears(7));
        var cvv = "123";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new PaymentDetails(null, expirationDate, cvv));
        
        // Assert
        Assert.Equal("cardNumber", exception.ParamName);
        Assert.Contains("Card number cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void CreatePaymentDetails_ExpiredCard_ShouldThrowArgumentException_WhenDateTimeInvalid()
    {
        // Arrange
        var cardNumber = "4242424242424242"; 
        var expirationDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1));
        var cvv = "123";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new PaymentDetails(cardNumber, expirationDate, cvv));
        
        // Assert
        Assert.Equal("expirationDate", exception.ParamName);
        Assert.Contains("Expiration date must be in the future.", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12")]
    [InlineData("1234")]
    [InlineData("abc")]
    [InlineData("12a")]
    [InlineData("!@#")]
    public void CreatePaymentDetails_InvalidCvv_ShouldThrowArgumentException(string cvv)
    {
        // Arrange
        var cardNumber = "4242424242424242"; 
        var expirationDate = DateOnly.FromDateTime(DateTime.Today.AddYears(7));
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new PaymentDetails(cardNumber, expirationDate, cvv));
        
        // Assert
        Assert.Equal("cvv", exception.ParamName);
        Assert.Contains("CVV must be a 3-digit number.", exception.Message);
    }
    
    [Theory]
    [InlineData("42424242424242424242")] // 20 digits
    [InlineData("424242424242424242424242")] // 24 digits
    [InlineData("123456789012")] // 12 digits
    [InlineData("1234567890")] // 10 digits
    [InlineData("1")] // 1 digit
    public void ValidateCardNumber_ShouldThrowArgumentException_WhenCardNumberIsTooSmallOrTooLarge(string cardNumber)
    {
        // Arrange
        var expirationDate = DateOnly.FromDateTime(DateTime.Today.AddYears(7));
        var cvv = "123";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new PaymentDetails(cardNumber, expirationDate, cvv));
        
        // Assert
        Assert.Equal("cardNumber", exception.ParamName);
        Assert.Contains("Card number must be between 13 and 19 digits.", exception.Message);
    }
    
    [Theory]
    [InlineData("1234567890123")] // Invalid Luhn
    [InlineData("1934567812345670")] // Invalid Luhn
    [InlineData("799273987108789")] // Invalid Luhn
    [InlineData("1234567890123456")] // Invalid Luhn
    [InlineData("12345678901234567")] // Invalid Luhn
    public void ValidateCardNumber_ShouldThrowArgumentException_WhenLuhnCheckFails(string cardNumber)
    {
        // Arrange
        var expirationDate = DateOnly.FromDateTime(DateTime.Today.AddYears(7));
        var cvv = "123";
        
        // Act
        var exception = Assert.Throws<ArgumentException>(() => 
            new PaymentDetails(cardNumber, expirationDate, cvv));
        
        // Assert
        Assert.Equal("cardNumber", exception.ParamName);
        Assert.Contains("Invalid card number.", exception.Message);
    }
}