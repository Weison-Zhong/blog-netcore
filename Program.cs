
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
//ע��DbContext����
builder.Services.AddDbContext<RoutineDbContext>(options =>
{
    options.UseMySql(keysBuilder["MySql"], MySqlServerVersion.LatestSupportedServerVersion);
});
//ע��ִ�����
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
#region Ȩ����֤���
//Jwt
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenModel = builder.Configuration.GetSection("Jwt").Get<JwtModel>();
        var secretByte = Encoding.UTF8.GetBytes(keysBuilder["Jwt"]);
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            //3+2��֤
            ValidateIssuer = true,//�Ƿ���֤������
            ValidIssuer = "issuer",//��Ҫ������tokenʱ��ֵһ��

            ValidateAudience = true, //�Ƿ���֤������
            ValidAudience = "audience",//��Ҫ������tokenʱ��ֵһ��

            ValidateLifetime = true, //�Ƿ���֤����ʱ��

            IssuerSigningKey = new SymmetricSecurityKey(secretByte)
        };
    });
//API��Ȩ
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
    //Ĭ����FALSE���ĳ�true������ͻ�������ͷAccept��������(����application/xml���ͷ���˿�֧�ֵ�����
    //������application/json����һ��ʱ�᷵��406
    setup.ReturnHttpNotAcceptable = true;
}).ConfigureApiBehaviorOptions(setup =>
{
    //�Զ��巵��״̬��
    setup.InvalidModelStateResponseFactory = context =>
    {
        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Title = "�����ˣ�",
            Status = StatusCodes.Status422UnprocessableEntity,
            Detail = "�뿴errors��ϸ��Ϣ",
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
    //Bearer ��scheme����
    var securityScheme = new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        //���������ͷ��
        In = ParameterLocation.Header,
        //ʹ��Authorizeͷ��
        Type = SecuritySchemeType.Http,
        //����Ϊ�� bearer��ͷ
        Scheme = "bearer",
        BearerFormat = "JWT"
    };
    //�����з�������Ϊ����bearerͷ����Ϣ
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

    //ע�ᵽswagger��
    c.AddSecurityDefinition("bearerAuth", securityScheme);
    c.AddSecurityRequirement(securityRequirement);
});



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//����jwt����UseAuthorizationǰ������һ��
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
