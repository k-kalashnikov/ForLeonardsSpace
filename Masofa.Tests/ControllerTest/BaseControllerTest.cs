using Masofa.BusinessLogic.Services.BusinessLogicLogger;

namespace Masofa.Tests.ControllerTest;

public abstract class BaseControllerTest
{
    protected Mock<ILogger> LoggerMock { get; set; } = null!;
    protected Mock<IMediator> MediatorMock { get; set; } = null!;
    protected Mock<IHttpContextAccessor> HttpContextAccessorMock { get; set; } = null!;
    protected Mock<IConfiguration> ConfigurationMock { get; set; } = null!;
    protected Mock<IBusinessLogicLogger> BusinessLogicLoggerMock { get; set; } = null!;

    // Для установки User.Identity.Name в запросах
    protected ClaimsPrincipal CreateTestUser(string userName = "testuser")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userName)
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
    }

    protected virtual void InitializeBaseMocks()
    {
        LoggerMock = new Mock<ILogger>();
        MediatorMock = new Mock<IMediator>();
        HttpContextAccessorMock = new Mock<IHttpContextAccessor>();
        ConfigurationMock = new Mock<IConfiguration>();
        BusinessLogicLoggerMock = new Mock<IBusinessLogicLogger>();

        // Настройка HttpContext для User
        var httpContext = new DefaultHttpContext();
        httpContext.User = CreateTestUser();
        HttpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Настройка IConfiguration
        ConfigurationMock.Setup(x => x[It.IsAny<string>()]).Returns((string)null);
    }

    protected BaseControllerTest()
    {
        InitializeBaseMocks();
    }

    protected TController CreateController<TController>(params object[] additionalDependencies)
        where TController : BaseController
    {
        var controller = (TController)Activator.CreateInstance(
            typeof(TController),
            LoggerMock.Object,
            ConfigurationMock.Object,
            MediatorMock.Object
        );

        var httpContext = new DefaultHttpContext();
        httpContext.User = CreateTestUser();
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        return controller;
    }
}