---
created: 2024-12-12T23:15:22 (UTC +08:00)
tags: [Vue3,webApi,流程,权限,从0到1 搭建OverallAuth2.0 权限管理系统,OverallAuth2.0 权限管理系统]
source: https://www.cnblogs.com/cyzf/p/18410483
author: 陈逸子风
---

# 从0到1搭建权限管理系统系列一 .net8 使用Swagger（附当前源码） - 陈逸子风 - 博客园

> ## Excerpt
> 说明 该文章是属于OverallAuth2.0系列文章，每周更新一篇该系列文章（从0到1完成系统开发）。 该系统文章，我会尽量说的非常详细，做到不管新手、老手都能看懂。 说明：OverallAuth2.0 是一个简单、易懂、功能强大的权限+可视化流程管理系统。 有兴趣的朋友，请关注我吧(*^▽^*)

---
**说明  
**

   该文章是属于OverallAuth2.0系列文章，每周更新一篇该系列文章（从0到1完成系统开发）。

    该系统文章，我会尽量说的非常详细，做到不管新手、老手都能看懂。

    说明：OverallAuth2.0 是一个简单、易懂、功能强大的权限+可视化流程管理系统。

**有兴趣的朋友，请关注我吧(\*^▽^\*)。**

**使用前提  
**

**1、Visual Studio使用2022版本  
**

**搭建项目  
**

    OverallAuth2.0依然和OverallAuth1.0一样，采用前后端分离模式，所以搭建后端，我们选择  .net core web api（如下图）

![](https://img2024.cnblogs.com/blog/1158526/202409/1158526-20240912161037135-1552827599.png)

    选择项目模板后，我们点击【下一步】

![](https://img2024.cnblogs.com/blog/1158526/202409/1158526-20240912161052220-911898265.png)

选择.net 8.0(最新长期支持版本)，随后创建项目

默认项目结构如下图

![](https://img2024.cnblogs.com/blog/1158526/202409/1158526-20240912161103533-47110754.png)

直接运行，查看默认界面

![](https://img2024.cnblogs.com/blog/1158526/202409/1158526-20240912161117958-1486706222.png)

    运行起来可以看到，系统默认的swagger界面非常简介，也少了很多信息比如：

1、系统说明、版本、作者等。

2、接口的描述、参数等信息。

3、接口的分类等。

**优化Swagger  
**

    上面说道，系统默认的接口文档是非常简洁的，接下来我们在系统中，这样做，让swagger看起来更优美和专业。

 在项目下新增一个文件件PlugInUnit，然后再该文件夹下新建一个类SwaggerPlugInUnit**。  
**

      建好SwaggerPlugInUnit后，在webapi同级建一个类库Utility，用于存放系统的辅助工具等，然后再该类库下建一个Enum文件夹，并新建ModeuleGroupEnum该枚举。

如图：

![](https://img2024.cnblogs.com/blog/1158526/202409/1158526-20240912161135787-838323720.png)

建好文件后，在ModeuleGroupEnum文件中写一个枚举SysMenu，并保存。

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
/// <summary>
/// 模块分组
/// </summary>
public enum ModeuleGroupEnum
{
    SysMenu = 1,
}
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

随后在SwaggerPlugInUnit中编写一个方法，具体代码如下

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
/// <summary>
/// swagger插件
/// </summary>
public static class SwaggerPlugInUnit
{
    /// <summary>
    /// 初始化Swagger
    /// </summary>
    /// <param name="services"></param>
    public static void InitSwagger(this IServiceCollection services)
    {
        //添加swagger
        services.AddSwaggerGen(optinos =>
        {
            typeof(ModeuleGroupEnum).GetEnumNames().ToList().ForEach(version =>
            {
                optinos.SwaggerDoc(version, new OpenApiInfo()
                {
                    Title = "权限管理系统",
                    Version = "V2.0",
                    Description = "求关注，求一键三连",
                    Contact = new OpenApiContact { Name = "微信公众号作者：不只是码农   b站作者：我不是码农呢", Url = new Uri("http://www.baidu.com") }
                });

            });

            //反射获取接口及方法描述
            var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            optinos.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName), true);

        });
    }

    /// <summary>
    /// swagger加入路由和管道
    /// </summary>
    /// <param name="app"></param>
    public static void InitSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            typeof(ModeuleGroupEnum).GetEnumNames().ToList().ForEach(versoin =>
            {
                options.SwaggerEndpoint($"/swagger/{versoin}/swagger.json", $"接口分类{versoin}");
            });
        });
    }
}


```
然后再program中使用自定义swagger中间件  
![](https://img2024.cnblogs.com/blog/1158526/202409/1158526-20240912161439487-2111244537.png)

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

   做好以上步骤，我们的swagger基本算是搭建好了，只需要在控制器上方，添加路由和分组。

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
 /// <summary>
 /// 系统模块
 /// </summary>
 [ApiController]
 [Route("api/[controller]/[action]")]
 [ApiExplorerSettings(GroupName = nameof(ModeuleGroupEnum.SysMenu))]
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

![](https://img2024.cnblogs.com/blog/1158526/202409/1158526-20240912161523755-2147131863.png)

做完以上这些，我们对swagger就算优化完成，只需要运行系统，就可以查看效果

![](https://img2024.cnblogs.com/blog/1158526/202409/1158526-20240912161541274-510956673.png)

**注意：**必须生成接口的xml文件，不然会报错。

![](https://img2024.cnblogs.com/blog/1158526/202409/1158526-20240912161552034-990154104.png)

  好了，以上就是搭建WebApi+优化Swagger的全部过程，你快来试试吧

**如果对你有帮助，请关注我吧(\*^▽^\*)。  
**

**源代码地址：https://gitee.com/yangguangchenjie/overall-auth2.0-web-api**  

**预览地址：http://139.155.137.144:8880/swagger/index.html**

**帮我Star，谢谢。**

**有兴趣的朋友，请关注我吧(\*^▽^\*)。**

**![](https://img2024.cnblogs.com/blog/1158526/202408/1158526-20240824140446786-404771438.png)**

关注我：一个全栈多端的宝藏博主，定时分享技术文章，不定时分享开源项目。关注我，带你认识不一样的程序世界
