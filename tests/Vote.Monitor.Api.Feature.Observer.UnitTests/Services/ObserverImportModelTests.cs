using Vote.Monitor.Api.Feature.Observer.Services;

namespace Vote.Monitor.Api.Feature.Observer.UnitTests.Services;

public class ObserverImportModelTests
{
    [Fact]
    public void GetHashCode_ReturnsSameHashCodeForEqualEmail()
    {
        // Arrange
        var observer1 = new ObserverImportModel { Name = "d", PhoneNumber = "00009999999", Email = "test@example.com" };
        var observer2 = new ObserverImportModel { Name = "d1", PhoneNumber = "11119999999", Email = "test@example.com" };

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
        var observer1 = new ObserverImportModel { Name = "d", PhoneNumber = "00009999999", Email = "test@example.com" };
        var observer2 = new ObserverImportModel { Name = "d1", PhoneNumber = "11119999999", Email = "test@example.com" };

        // Act
        var areEqual = observer1.Equals(observer2);

        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void Equals_ReturnsTrueForSameInstance()
    {
        // Arrange
        var observer = new ObserverImportModel { Name = "d", PhoneNumber = "00009999999", Email = "test@example.com" };

        // Act
        var areEqual = observer.Equals(observer);

        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void Equals_ReturnsFalseForDifferentEmails()
    {
        // Arrange
        var observer1 = new ObserverImportModel { Name = "d", PhoneNumber = "00009999999", Email = "test@example.com" };
        var observer2 = new ObserverImportModel { Name = "d1", PhoneNumber = "11119999999", Email = "test1@example.com" };

        // Act
        var areEqual = observer1.Equals(observer2);

        // Assert
        Assert.False(areEqual);
    }

    [Fact]
    public void Equals_ReturnsFalseForNullInstance()
    {
        // Arrange
        var observer = new ObserverImportModel { Name = "d", PhoneNumber = "00009999999", Email = "test@example.com" };

        // Act
        var areEqual = observer.Equals(null);

        // Assert
        Assert.False(areEqual);
    }

    [Fact]
    public void Equals_ReturnsFalseForDifferentType()
    {
        // Arrange
        var observer = new ObserverImportModel { Name = "d", PhoneNumber = "00009999999", Email = "test@example.com" };
        var otherObject = new object();

        // Act
        var areEqual = observer.Equals(otherObject);

        // Assert
        Assert.False(areEqual);
    }
}

