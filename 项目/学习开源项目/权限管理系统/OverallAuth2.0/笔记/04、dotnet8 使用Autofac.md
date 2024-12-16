---
created: 2024-12-16T10:07:25 (UTC +08:00)
tags: [Vue3,webApi,表单,流程,权限,从0到1 搭建OverallAuth2.0 权限管理系统,OverallAuth2.0 权限管理系统]
source: https://www.cnblogs.com/cyzf/p/18439606
author: 陈逸子风
---

# 从0到1搭建权限管理系统系列四 .net8 中Autofac的使用（附源码） - 陈逸子风 - 博客园

> ## Excerpt
> 说明 该文章是属于OverallAuth2.0系列文章，每周更新一篇该系列文章（从0到1完成系统开发）。 该系统文章，我会尽量说的非常详细，做到不管新手、老手都能看懂。 说明：OverallAuth2.0 是一个简单、易懂、功能强大的权限+可视化流程管理系统。 友情提醒：本篇文章是属于系列文章，看该

---
**说明  
**

    该文章是属于OverallAuth2.0系列文章，每周更新一篇该系列文章（从0到1完成系统开发）。

    该系统文章，我会尽量说的非常详细，做到不管新手、老手都能看懂。

    说明：OverallAuth2.0 是一个简单、易懂、功能强大的权限+可视化流程管理系统。

友情提醒：本篇文章是属于系列文章，看该文章前，建议先看之前文章，可以更好理解项目结构。

**有兴趣的朋友，请关注我吧(\*^▽^\*)。**

**![](https://img2024.cnblogs.com/blog/1158526/202408/1158526-20240824140446786-404771438.png)**

**关注我，学不会你来打我**

**关于Autofac**

虽然微软有自带的Ioc框架，但是我想说的是Autofac应该说是我用过的最好的Ioc框架。

它的高性能、灵活性、对Aop的支持，简直就是开发人员的梦中情人。

autofac，它能有效的降低繁杂系统中，代码的耦合度，提高代码的可维护性。

不仅如此，它可以轻松替换依赖关系，从而提高测试效率。

**安装Autofac**

1、在启动项目中，安装最新版Autofac和Autofac.Extensions.DependencyInjection。

**创建类库**

1、DomainService：领域服务类

2、Infrastructure：基础设施类

3、CoreDomain：核心领域类

如果你看了该系列每篇文章，你会发现，我们的底层架构，是使用DDD领域驱动模型的设计模式搭建后端。

系统架构如图所示：

![](https://img2024.cnblogs.com/blog/1158526/202409/1158526-20240929145939845-1770896034.png)

搭建好系统架构，我们开始使用Autofac

**创建AutofacPlugIn类文件**

在系统PlugIn文件中创建AutofacPlugIn类文件，内容如下：

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
/// <summary>
/// Autofac插件
/// </summary>
public class AutofacPlugIn : Autofac.Module
{
    /// <summary>
    /// 重写Autofac的Load方法
    /// </summary>
    /// <param name="containerBuilder"></param>
    protected override void Load(ContainerBuilder containerBuilder)
    {
        //服务项目程序集
        Assembly service = Assembly.Load("DomainService");
        Assembly intracface = Assembly.Load("Infrastructure");

        //项目必须以xxx结尾
        containerBuilder.RegisterAssemblyTypes(service).Where(n => n.Name.EndsWith("Service") && !n.IsAbstract)
            .InstancePerLifetimeScope().AsImplementedInterfaces();
        containerBuilder.RegisterAssemblyTypes(intracface).Where(n => n.Name.EndsWith("Repository") && !n.IsAbstract)
           .InstancePerLifetimeScope().AsImplementedInterfaces();

    }

}
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

代码解释：

获取DomainService和Infrastructure程序集，然后批量注入。

值得注意的是，我们在创建类时，给了限定条件。那就是程序集下面的类，必须以Service和Repository结尾，否则autofac会抛出错误。

当然有以xxx结尾，那必然有以xxx开头。使用StartsWith轻松解决。

**在Program组成Autofac**

.net8 autofac注册和以往有点出去，需要在Program中添加如如下代码

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
      //注册Autofac
      builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(builder =>
      {
          builder.RegisterModule(new AutofacPlugIn());
      });
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

经过以上配置，autofac已经配置好，是不是很简单，接下来测试一下。

**使用**

在Infrastructure类中建立ISysUserRepository和SysUserRepository仓储接口及接口实现

结构如下：

![](https://img2024.cnblogs.com/blog/1158526/202409/1158526-20240929153733481-501592113.png)

内容代码如下：

ISysUserRepository

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
    /// <summary>
    /// 用户服务仓储接口
    /// </summary>
    public interface ISysUserRepository
    {
        /// <summary>
        /// Autofac测试
        /// </summary>
        /// <returns></returns>
        string TestAutofac();
    }
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

SysUserRepository

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
  /// <summary>
  /// 用户仓储服务接口实现
  /// </summary>
  internal class SysUserRepository : ISysUserRepository
  {
      /// <summary>
      /// 测试Autofac
      /// </summary>
      /// <returns></returns>
      public string TestAutofac()
      {
          return "Autofac使用成功！";
      }
  }
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

在DomainService中创建ISysUserService和SysUserService服务接口和实现

结构如下：

![](https://img2024.cnblogs.com/blog/1158526/202409/1158526-20240929154035529-138006509.png)

内容代码如下

ISysUserService

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
   /// <summary>
   /// 用户服务接口
   /// </summary>
  public interface ISysUserService
   {
       /// <summary>
       /// 测试autofac
       /// </summary>
       /// <returns></returns>
       string TestAutofac();
   }
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

SysUserService

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
   internal class SysUserService : ISysUserService
   {
       #region 构造实例化
       public readonly ISysUserRepository _sysUserRepository;
       public SysUserService(ISysUserRepository sysUserRepository)
       {
           _sysUserRepository = sysUserRepository;
       }
       #endregion

       /// <summary>
       /// 测试TestAutofac
       /// </summary>
       /// <returns></returns>
       public string TestAutofac()
       {
           return _sysUserRepository.TestAutofac();
       }
   }
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

**测试**

在SysUserController控制器中调用接口（该控制器，在之前文章中已经建好，没看之前文章的，随意见一个webapi的控制器也行）。

代码如下：

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
  /// <summary>
  /// 用户模块
  /// </summary>
  [ApiController]
  [Route("api/[controller]/[action]")]
  [ApiExplorerSettings(GroupName = nameof(ModeuleGroupEnum.SysUser))]
  public class SysUserController : BaseController
  {

      #region 构造实例化
      /// <summary>
      /// 用户服务
      /// </summary>
      public readonly ISysUserService _sysUserService;

      /// <summary>
      /// 构造函数
      /// </summary>
      /// <param name="sysUserService">用户服务</param>
      public SysUserController(ISysUserService sysUserService)
      {
          _sysUserService = sysUserService;
      }
      #endregion

      /// <summary>
      /// 测试Autofac
      /// </summary>
      /// <returns></returns>
      [HttpGet]
      [AllowAnonymous]
      public String TestAutofac()
      {
          return _sysUserService.TestAutofac();
      }

      /// <summary>
      /// 获取Token
      /// </summary>
      /// <param name="userName">用户名</param>
      /// <param name="password">密码</param>
      [HttpGet]
      [AllowAnonymous]
      public string GetToken(string userName, string password)
      {
          var loginResult = JwtPlugInUnit.BuildToken(new LoginInput { UserName = userName, Password = password });

          return loginResult.Token ?? string.Empty;
      }
  }
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

上述代码中要先通过Autofac来实例化接口。

打开swagger，开始测试

![](https://img2024.cnblogs.com/blog/1158526/202409/1158526-20240929154819720-188455329.png)

之前说道，我们的接口必须以xxx结束，那么现在我们把这个约定改一下，看能否调用成功。

修改代码如下：

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
/// <summary>
 /// 重写Autofac的Load方法
 /// </summary>
 /// <param name="containerBuilder"></param>
 protected override void Load(ContainerBuilder containerBuilder)
 {
     //服务项目程序集
     Assembly service = Assembly.Load("DomainService");
     Assembly intracface = Assembly.Load("Infrastructure");

     //项目必须以xxx结尾
     containerBuilder.RegisterAssemblyTypes(service).Where(n => n.Name.EndsWith("Service111111111") && !n.IsAbstract)
         .InstancePerLifetimeScope().AsImplementedInterfaces();
     containerBuilder.RegisterAssemblyTypes(intracface).Where(n => n.Name.EndsWith("Repository2222222") && !n.IsAbstract)
        .InstancePerLifetimeScope().AsImplementedInterfaces();

 }
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

可以看到，我们把以Service、Repository结束改成了Service111111111、Repository2222222，也就是说，我们的领域服务类和基础仓储类是不符合改命名规范的，aufofac应该报出错误。

测试：

![](https://img2024.cnblogs.com/blog/1158526/202409/1158526-20240929155308650-408790733.png)

以上就是aufofac在本期内容中的使用。

在接下来的系列中，会使用autofac+aop的结合使用，有兴趣的朋友，请博客园关注、微信关注，感谢观看。

**源代码地址：https://gitee.com/yangguangchenjie/overall-auth2.0-web-api**  

****预览地址：http://139.155.137.144:8880/swagger/index.html****

**帮我Star，谢谢。**

**有兴趣的朋友，请关注我微信公众号吧(\*^▽^\*)。**

**![](https://img2024.cnblogs.com/blog/1158526/202408/1158526-20240824140446786-404771438.png)**

关注我：一个全栈多端的宝藏博主，定时分享技术文章，不定时分享开源项目。关注我，带你认识不一样的程序世界
