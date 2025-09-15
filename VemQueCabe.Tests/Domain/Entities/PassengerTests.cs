using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Tests.Domain.Entities;

public class PassengerTests
{
    [Fact]
    public void CreatePassenger_ValidParameters_ShouldCreatePassenger()
    {
        // Arrange
        var userId = 1;
        var paymentDetails = new PaymentDetails("4242424242424242", DateOnly.Parse("2035-08-08"), "123");
        
        // Act
        var passenger = new Passenger(userId, paymentDetails);
        
        // Assert
        Assert.Equal(userId, passenger.UserId);
        Assert.Equal(paymentDetails, passenger.PaymentInformation);
        Assert.False(passenger.HasActiveRequest);
    }

    [Fact]
    public void CreatePassenger_InvalidUserId_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var userId = -1;
        var paymentDetails = new PaymentDetails("4242424242424242", DateOnly.Parse("2035-08-08"), "123");

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => 
            new Passenger(userId, paymentDetails));
        
        // Assert
        Assert.Equal("userId", exception.ParamName);
        Assert.Contains("User ID must be greater than zero.", exception.Message);
    }

    [Fact]
    public void CreatePassenger_NullPaymentDetails_ShouldThrowArgumentNullException()
    {
        // Arrange
        var userId = 1;
        
        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new Passenger(userId, null));
        
        // Assert
        Assert.Equal("paymentInformation", exception.ParamName);
        Assert.Contains("Payment information cannot be null.", exception.Message);
    }

    [Fact]
    public void SetHasActiveRequest_ValidChange_ShouldUpdateRequestStatus()
    {
        // Arrange
        var userId = 1;
        var paymentDetails = new PaymentDetails("4242424242424242", DateOnly.Parse("2035-08-08"), "123");
        var passenger = new Passenger(userId, paymentDetails);
        var activeRequest = true;
        
        // Act
        passenger.SetHasActiveRequest(activeRequest);
        
        // Assert
        Assert.Equal(activeRequest, passenger.HasActiveRequest);
    }

    [Fact]
    public void SetHasActiveRequest_SameValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = 1;
        var paymentDetails = new PaymentDetails("4242424242424242", DateOnly.Parse("2035-08-08"), "123");
        var passenger = new Passenger(userId, paymentDetails);
        
        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => 
            passenger.SetHasActiveRequest(false));
        
        // Assert
        Assert.Equal("Request status is already set to the requested value.", exception.Message);
    }

    [Fact]
    public void UpdatePaymentInformation_ValidUpdate_ShouldChangePaymentDetails()
    {
        // Arrange
        var userId = 1;
        var paymentDetails = new PaymentDetails("4242424242424242", DateOnly.Parse("2035-08-08"), "123");
        var passenger = new Passenger(userId, paymentDetails);
        var newPaymentDetails = new PaymentDetails("5555555555554444", DateOnly.Parse("2040-10-10"), "456");
        
        // Act
        passenger.UpdatePaymentInformation(newPaymentDetails);
        
        // Assert
        Assert.Equal(newPaymentDetails, passenger.PaymentInformation);
    }

    [Fact]
    public void UpdatePaymentInformation_NullPaymentDetails_ShouldThrowArgumentNullException()
    {
        // Arrange
        var userId = 1;
        var paymentDetails = new PaymentDetails("4242424242424242", DateOnly.Parse("2035-08-08"), "123");
        var passenger = new Passenger(userId, paymentDetails);
        
        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => 
            passenger.UpdatePaymentInformation(null));
        
        // Assert
        Assert.Equal("paymentDetails", exception.ParamName);
        Assert.Contains("Payment information cannot be null.", exception.Message);
    }
}