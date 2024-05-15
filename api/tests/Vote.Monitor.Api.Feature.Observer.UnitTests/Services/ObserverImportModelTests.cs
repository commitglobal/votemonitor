using Vote.Monitor.Api.Feature.Observer.Parser;

namespace Vote.Monitor.Api.Feature.Observer.UnitTests.Services;

public class ObserverImportModelTests
{
    [Fact]
    public void GetHashCode_ReturnsSameHashCodeForEqualEmail()
    {
        // Arrange
        var observer1 = new ObserverImportModel
        {
            FirstName = "d",
            LastName = "LastName1",
            PhoneNumber = "00009999999",
            Email = "test@example.com",
            Password = "Password1!"
        };

        var observer2 = new ObserverImportModel
        {
            FirstName = "d1",
            PhoneNumber = "11119999999",
            Email = "test@example.com",
            LastName = "LastName2",
            Password = "Password2!"
        };

        // Act
        var hashCode1 = observer1.GetHashCode();
        var hashCode2 = observer2.GetHashCode();

        // Assert
        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void Equals_ReturnsTrueForEqualEmail()
    {
        // Arrange
        var observer1 = new ObserverImportModel
        {
            PhoneNumber = "00009999999",
            Email = "test@example.com",
            LastName = "LastName1",
            FirstName = "FirstName1",
            Password = "Password!1"
        };
        var observer2 = new ObserverImportModel
        {
            PhoneNumber = "1111111",
            Email = "test@example.com",
            LastName = "LastName2",
            FirstName = "FirstName2",
            Password = "Password!2"
        };

        // Act
        var areEqual = observer1.Equals(observer2);

        // Assert
        Assert.True(areEqual);
    }

   
    [Fact]
    public void Equals_ReturnsFalseForDifferentEmails()
    {
        // Arrange
        var observer1 = new ObserverImportModel
        {
            PhoneNumber = "00009999999",
            Email = "test@example.com",
            LastName = "LastName",
            FirstName = "FirstName",
            Password = "Password"
        };
        var observer2 = new ObserverImportModel
        {
            PhoneNumber = "00009999999",
            Email = "noop@example.com",
            LastName = "LastName",
            FirstName = "FirstName",
            Password = "Password"
        };

        // Act
        var areEqual = observer1.Equals(observer2);

        // Assert
        Assert.False(areEqual);
    }
}

