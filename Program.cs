
using Blog2022_netcore.Common;
using Blog2022_netcore.Data;
using Blog2022_netcore.Policy;
using Blog2022_netcore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5000;");
var keysBuilder = new ConfigurationBuilder().AddJsonFile("keys.json").Build();
//注册DbContext服务
builder.Services.AddDbContext<RoutineDbContext>(options =>
{
    options.UseMySql(keysBuilder["MySql"], MySqlServerVersion.LatestSupportedServerVersion);
});
//注册仓储服务
builder.Services.AddScoped<IAdministratorRepository, AdministratorRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IApiRepository, ApiRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IGuestRepository, GuestRepository>();
builder.Services.AddScoped<IIconRepository, IconRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IDemoRepository, DemoRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddHttpContextAccessor();
//builder.Services.AddHttpClient();
#region 权限认证相关
//Jwt
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenModel = builder.Configuration.GetSection("Jwt").Get<JwtModel>();
        var secretByte = Encoding.UTF8.GetBytes(keysBuilder["Jwt"]);
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            //3+2认证
            ValidateIssuer = true,//是否认证发行人
            ValidIssuer = "issuer",//需要和生成token时的值一样

            ValidateAudience = true, //是否认证订阅人
            ValidAudience = "audience",//需要和生成token时的值一样

            ValidateLifetime = true, //是否认证过期时间

            IssuerSigningKey = new SymmetricSecurityKey(secretByte)
        };
    });
//API授权
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiPolicy", o =>
     {
         o.Requirements.Add(new PermissionRequirement());
     });
});

#endregion


builder.Services.AddControllers(setup =>
{
    //默认是FALSE，改成true后如果客户端请求头Accept请求类型(例如application/xml）和服务端可支持的类型
    //（例如application/json）不一致时会返回406
    setup.ReturnHttpNotAcceptable = true;
}).ConfigureApiBehaviorOptions(setup =>
{
    //自定义返回状态码
    setup.InvalidModelStateResponseFactory = context =>
    {
        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Title = "出错了！",
            Status = StatusCodes.Status422UnprocessableEntity,
            Detail = "请看errors详细信息",
            Instance = context.HttpContext.Request.Path
        };
        problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
        return new UnprocessableEntityObjectResult(problemDetails)
        {
            ContentTypes = { "application/problem+json" }
        };
    };
});

builder.Services.AddSwaggerGen(c =>
{
    //Bearer 的scheme定义
    var securityScheme = new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        //参数添加在头部
        In = ParameterLocation.Header,
        //使用Authorize头部
        Type = SecuritySchemeType.Http,
        //内容为以 bearer开头
        Scheme = "bearer",
        BearerFormat = "JWT"
    };
    //把所有方法配置为增加bearer头部信息
    var securityRequirement = new OpenApiSecurityRequirement
     {
      {
        new OpenApiSecurityScheme
           {
             Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "bearerAuth"
                                    }
                                },
                                new string[] {}
                        }
                    };

    //注册到swagger中
    c.AddSecurityDefinition("bearerAuth", securityScheme);
    c.AddSecurityRequirement(securityRequirement);
});



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//加了jwt后在UseAuthorization前加下面一行
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
