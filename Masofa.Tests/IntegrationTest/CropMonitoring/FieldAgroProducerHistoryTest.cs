using FluentAssertions;
using Masofa.Client.ApiClient;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.ViewModels.Account;
using Xunit.Abstractions;

namespace Masofa.Tests.IntegrationTest.CropMonitoring;

[Collection("Sequential")]
public class FieldAgroProducerHistoryTest
{
    private readonly ITestOutputHelper _output;
    private readonly string _baseUrl;
    private string? _authToken;
    private HttpClient? _sharedHttpClient;

    public FieldAgroProducerHistoryTest(ITestOutputHelper output)
    {
        _output = output;
        _baseUrl = TestConstants.BASE_URL;
        _sharedHttpClient = new HttpClient();
    }

    #region GetByQuery

    [Fact]
    public async Task GetByQueryAsync_WithValidQuery_ShouldReturnListOfFieldAgroProducerHistory()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
        await unitOfWork.LoginAsync(new LoginAndPasswordViewModel { UserName = TestConstants.TEST_USERNAME, Password = TestConstants.TEST_PASSWORD }, CancellationToken.None);

        var fieldAgroProducerHistoryRepository = unitOfWork.FieldAgroProducerHistoryRepository;

        var query = new BaseGetQuery<FieldAgroProducerHistory>
        {
            Take = 5,
            Offset = 0,
            Sort = SortType.ASC,
            SortBy = "Id"
        };

        _output.WriteLine("Executing Standard GetByQueryAsync with a query to get up to 5 FieldAgroProducerHistories...");
        try
        {
            // Act
            var result = await fieldAgroProducerHistoryRepository.GetByQueryAsync(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("Result list should not be null.");
            result.Should().BeAssignableTo<List<FieldAgroProducerHistory>>("Result should be a list of FieldAgroProducerHistory.");
            result.Count.Should().BeLessThanOrEqualTo(5, "Query requested up to 5 items.");
            if (result.Count > 0)
            {
                var firstFieldAgroProducerHistory = result[0];
                firstFieldAgroProducerHistory.Id.Should().NotBeEmpty("Each FieldAgroProducerHistory should have a valid ID.");
            }
            _output.WriteLine($"Standard GetByQueryAsync test completed successfully. Retrieved {result.Count} FieldAgroProducerHistories.");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Exception: {ex}");
            _output.WriteLine($"InnerException: {ex.InnerException}");
            throw;
        }
    }

    #endregion

}
