using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Utility;

namespace ProjectManageWebApi
{
    /// <summary>
    /// ������
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// ��������
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// ���÷���
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //�ӿ��ĵ����˵��
            services.AddSwaggerGen(options =>
            {
                // ע��
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // �ڶ�������Ϊ�Ƿ���ʾ������ע��,����ѡ��true
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    Type = SecuritySchemeType.ApiKey
                });

                // ���ɶ���ĵ���ʾ
                //typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                //{
                //    //����ĵ�����
                //    options.SwaggerDoc(version, new OpenApiInfo
                //    {
                //        Title = $"��Ŀ��",
                //        Version = version,
                //        Description = $"��Ŀ��:{version}�汾"
                //    });
                //});
            });

            //����һ��������Դ���Կ���
            services.AddCors(options =>
            {
                options.AddPolicy("CustomCorsPolicy", policy =>
                {
                    // �趨����������Դ���ж��������','����
                    policy.WithOrigins(Configuration.GetValue<string>("CustomCorsPolicy:WhiteList").Split(','))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });

            //���ȫ���쳣�������
            services.AddMvc(option =>
            {
                option.Filters.Add<ExceptionFilter>();
            });

            //ʱ���ʽ����
            services.AddControllers().AddJsonOptions(configure =>
            {
                configure.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());

            });

            //jwt��Ȩ
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, option =>
            {
                var jwtsetting = ConfigurationFileHelper.GetNode<JwtSetting>("JwtSetting");
                Configuration.Bind("JwtSetting", jwtsetting);
                option.SaveToken = true;
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = jwtsetting.Issuer,//������
                    ValidAudience = jwtsetting.Audience,//������
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsetting.SecurityKey)),//���ܵ���Կ
                    ValidateIssuerSigningKey = true,//�Ƿ���֤ǩ��,����֤�Ļ����Դ۸����ݣ�����ȫ
                    ValidateIssuer = true,//�Ƿ���֤�����ˣ�������֤�غ��е�Iss�Ƿ��ӦValidIssuer����
                    ValidateAudience = true,//�Ƿ���֤�����ˣ�������֤�غ��е�Aud�Ƿ��ӦValidAudience����
                    ValidateLifetime = true,//�Ƿ���֤����ʱ�䣬�����˾;ܾ�����
                    ClockSkew = TimeSpan.Zero,//����ǻ������ʱ�䣬Ҳ����˵����ʹ���������˹���ʱ�䣬����ҲҪ���ǽ�ȥ������ʱ��+���壬Ĭ�Ϻ�����7���ӣ������ֱ������Ϊ0
                    //RequireExpirationTime = true,
                };

                //option.Events = new JwtBearerEvent();
            });
        }

        /// <summary>
        /// Autofacע�����ĵط�,Autofac���Զ�����
        /// </summary>
        /// <param name="containerBuilder"></param>
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            //��������ע��ʾ��
            //   builder.RegisterType<MenuService>().As<IMenuService>();
            //    builder.RegisterType<IUserService>().As<IUserService>();

            //������Ŀ����
            Assembly service = Assembly.Load("Domain");
            Assembly intracface = Assembly.Load("Infrastructure");

            //��Ŀ������xxx��β
            containerBuilder.RegisterAssemblyTypes(service).Where(n => n.Name.EndsWith("Service") && !n.IsAbstract)
                .InstancePerLifetimeScope().AsImplementedInterfaces();
            containerBuilder.RegisterAssemblyTypes(intracface).Where(n => n.Name.EndsWith("Repository") && !n.IsAbstract)
               .InstancePerLifetimeScope().AsImplementedInterfaces();
        }

        /// <summary>
        /// ���öԽ�
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectManageWebApi v1"));
            //}
            // �趨�ض�ip������� CustomCorsPolicy����ConfigureServices���������õĿ����������
            app.UseCors("CustomCorsPolicy");

            //·���м��
            app.UseRouting();
            //��֤�м��
            app.UseAuthentication();
            //��Ȩ�м��
            app.UseAuthorization();
            //�ս��
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute()
                    .RequireAuthorization();
            });

        }
    }
}
