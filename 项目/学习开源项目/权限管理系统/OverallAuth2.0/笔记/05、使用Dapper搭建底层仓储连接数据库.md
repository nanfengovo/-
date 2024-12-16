---
created: 2024-12-16T15:00:31 (UTC +08:00)
tags: [Vue3,webApi,流程,权限,从0到1 搭建OverallAuth2.0 权限管理系统,OverallAuth2.0 权限管理系统]
source: https://www.cnblogs.com/cyzf/p/18448855
author: 陈逸子风
---

# （系列五）.net8 中使用Dapper搭建底层仓储连接数据库（附源码） - 陈逸子风 - 博客园

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

**安装Dapper**

1、在使用的地方，安装最新版Dapper。

2、在使用的地方，安装最新版的Microsoft.Extensions.Configuration

3、在使用的地方，安装最新版的Microsoft.Extensions.Configuration.Json

4、在使用的地方，安装最新版的System.Data.SqlClient

最终安装包如下：

![](https://img2024.cnblogs.com/blog/1158526/202410/1158526-20241006095559599-834353648.png)

**创建数据库连接类**

在创建数据库连接类之前，我们要先在appsettings.json中配置我们的数据库连接字符串。

```
SqlConnection<span>"</span><span>: </span><span>"</span>Server=SQLOLEDB;Data Source=你的sqlServer名称;uid=你的数据库账号;pwd=你的数据库密码;DataBase=你的数据库名字
```

如下图：

![](https://img2024.cnblogs.com/blog/1158526/202410/1158526-20241006094945407-54747879.png)

配置好数据库连接之后，我们需要一个读取数据库的类，所以我们需要在基础设施层（Infrastructure）创建。

创建文件夹DataBaseConnect然后在创建一个类DataBaseConnectConfig.cs。

内容如下：

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Data;
using System.Data.SqlClient;

namespace Infrastructure.DataBaseConnect
{
    /// <summary>
    /// 数据库连接类
    /// </summary>
    public static class DataBaseConnectConfig
    {
        /// <summary>
        /// 声明静态连接
        /// </summary>
        public static IConfiguration Configuration { get; set; }

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static DataBaseConnectConfig()
        {
            //ReloadOnChange = true 当appsettings.json被修改时重新加载            
            Configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="sqlConnectionStr">连接数据库字符串</param>
        /// <returns></returns>
        public static SqlConnection GetSqlConnection(string? sqlConnectionStr = null)
        {

            if (string.IsNullOrWhiteSpace(sqlConnectionStr))
            {
                sqlConnectionStr = Configuration["ConnectionStrings:SqlConnection"];
            }
            var connection = new SqlConnection(sqlConnectionStr);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }
    }
}
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
GetSqlConnection方法会读取appsettings.json中的连接配置，并打开数据库。
```

**创建仓储**

有了数据库连接类后，我们就要开始着手搭建底层仓储，结构如下：

![](https://img2024.cnblogs.com/blog/1158526/202410/1158526-20241006095908540-262958232.png)

根据以上结构，我们分别创建IRepository.cs仓储接口和Repository.cs仓储接口的实现

IRepository.cs内容如下：

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
/// <summary>
 /// 仓储接口定义
 /// </summary>
 public interface IRepository
 {
 }
 /// <summary>
 /// 定义泛型仓储接口
 /// </summary>
 /// <typeparam name="T">实体类型</typeparam>
 /// <typeparam name="object">主键类型</typeparam>
 public interface IRepository<T> : IRepository where T : class, new()
 {
     /// <summary>
     /// 新增
     /// </summary>
     /// <param name="entity">实体</param>
     /// <param name="innserSql">新增sql</param>
     /// <returns></returns>
     int Insert(T entity, string innserSql);

     /// <summary>
     /// 修改
     /// </summary>
     /// <param name="entity">实体</param>
     /// <param name="updateSql">更新sql</param>
     /// <returns></returns>
     int Update(T entity, string updateSql);

     /// <summary>
     /// 删除
     /// </summary>
     /// <param name="deleteSql">删除sql</param>
     /// <returns></returns>
     int Delete(string key, string deleteSql);

     /// <summary>
     /// 根据主键获取模型
     /// </summary>
     /// <param name="key">主键</param>
     /// <param name="selectSql">查询sql</param>
     /// <returns></returns>
     T GetByKey(string key, string selectSql);

     /// <summary>
     /// 获取所有数据
     /// </summary>
     /// <param name="selectAllSql">查询sql</param>
     /// <returns></returns>
     List<T> GetAll(string selectAllSql);

     /// <summary>
     /// 根据唯一主键验证数据是否存在
     /// </summary>
     /// <param name="id">主键</param>
     /// <param name="selectSql">查询sql</param>
     /// <returns>返回true存在，false不存在</returns>
     bool IsExist(string id, string selectSql);
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

说明1：该仓储接口是我们常用的crud（增删改查）的接口，它适用于所有的表结构，让我们不再重复的编写一样的sql语句。

说明2：该仓储接口可以自定义通用的接口，列如分页查询、批量新增、批量修改等。

Repository.cs结构如下

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
/// <summary>
 /// 仓储基类
 /// </summary>
 /// <typeparam name="T">实体类型</typeparam>
 /// <typeparam name="TPrimaryKey">主键类型</typeparam>
 public abstract class Repository<T> : IRepository<T> where T : class, new()
 {
     /// <summary>
     /// 删除
     /// </summary>
     /// <param name="deleteSql">删除sql</param>
     /// <returns></returns>
     public int Delete(string key, string deleteSql)
     {
         using var connection = DataBaseConnectConfig.GetSqlConnection();
         return connection.Execute(deleteSql, new { Key = key });
     }

     /// <summary>
     /// 根据主键获取模型
     /// </summary>
     /// <param name="id">主键</param>
     /// <param name="selectSql">查询sql</param>
     /// <returns></returns>
     public T GetByKey(string id, string selectSql)
     {
         using var connection = DataBaseConnectConfig.GetSqlConnection();
         return connection.QueryFirstOrDefault<T>(selectSql, new { Key = id });
     }

     /// <summary>
     /// 获取所有数据
     /// </summary>
     /// <param name="selectAllSql">查询sql</param>
     /// <returns></returns>
     public List<T> GetAll(string selectAllSql)
     {
         using var connection = DataBaseConnectConfig.GetSqlConnection();
         return connection.Query<T>(selectAllSql).ToList();
     }

     /// <summary>
     /// 新增
     /// </summary>
     /// <param name="entity">新增实体</param>
     /// <param name="innserSql">新增sql</param>
     /// <returns></returns>
     public int Insert(T entity, string innserSql)
     {
         using var connection = DataBaseConnectConfig.GetSqlConnection();
         return connection.Execute(innserSql, entity);
     }

     /// <summary>
     /// 根据唯一主键验证数据是否存在
     /// </summary>
     /// <param name="id">主键</param>
     /// <param name="selectSql">查询sql</param>
     /// <returns>返回true存在，false不存在</returns>
     public bool IsExist(string id, string selectSql)
     {
         using var connection = DataBaseConnectConfig.GetSqlConnection();
         var count = connection.QueryFirst<int>(selectSql, new { Key = id });
         if (count > 0)
             return true;
         else
             return false;
     }

     /// <summary>
     /// 更新
     /// </summary>
     /// <param name="entity">更新实体</param>
     /// <param name="updateSql">更新sql</param>
     /// <returns></returns>
     public int Update(T entity, string updateSql)
     {
         using var connection = DataBaseConnectConfig.GetSqlConnection();
         return connection.Execute(updateSql, entity);
     }
 }
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

该类是IRepository.cs仓储接口的实现，它继承于Repository.cs

**创建基础sql仓储（可省略）**

做完以上操作，我们的底层仓储事实上已经搭建完成，但博主搭建了一个基础sql的仓储，以便管理项目中所有基础的sql语句。

在Infrastructure的根目录创建一个BaseSqlRepository.cs的类。

编写sql的基础语句，现有语句如下。

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
/// <summary>
 /// 创建继承sql仓储
 /// </summary>
 public class BaseSqlRepository
 {
     #region 表Sys_user

     /// <summary>
     /// sys_user新增
     /// </summary>
     public static string sysUser_insertSql = @"insert into Sys_User (UserName ,Password ,Age,Sex,IsOpen,DepartmentId,CreateTime,CreateUser) values(@UserName ,@Password ,@Age,@Sex,@IsOpen,@DepartmentId,@CreateTime,@CreateUser)";

     /// <summary>
     /// sys_user更新
     /// </summary>
     public static string sysUser_updateSql = @"update Sys_User set UserName=@UserName ,Password=@Password ,Age=@Age,Sex=@Sex,DepartmentId=@DepartmentId,CreateTime=@CreateTime,CreateUser=@CreateUser where UserId = @UserId";

     /// <summary>
     /// sys_user查询
     /// </summary>
     public static string sysUser_selectByKeySql = @" select * from Sys_User where  UserId=@Key";

     /// <summary>
     /// sys_user表查询全部语句
     /// </summary>
     public static string sysUser_selectAllSql = @" select * from Sys_User";

     #endregion
 }
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

**创建表Sys\_User模型**

结构如下：

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
/// <summary>
/// 用户表模型
/// </summary>
public class SysUser
{
    /// <summary>
    /// 用户id
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 年龄
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public int Sex { get; set; }

    /// <summary>
    /// 是否开启
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// 部门id
    /// </summary>
    public int DepartmentId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 创建人员
    /// </summary>
    public string CreateUser { get; set; }
}
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

该结构，同数据库表结构（一会说）

**使用仓储**

在之前的ISysUserRepository.cs和SysUserRepository.cs中分别继承IRepository.cs 和Repository.cs

**![](https://img2024.cnblogs.com/blog/1158526/202410/1158526-20241006102900684-371596900.png)**

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
/// <summary>
/// 用户服务仓储接口
/// </summary>
public interface ISysUserRepository : IRepository<SysUser>
{
    /// <summary>
    /// 测试Autofac
    /// </summary>
    /// <returns></returns>
    string TestAutofac();
}
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
/// <summary>
 /// 用户服务仓储接口实现
 /// </summary>
 public class SysUserRepository : Repository<SysUser>, ISysUserRepository
 {
     /// <summary>
     /// 测试Autofac
     /// </summary>
     /// <returns></returns>
     public string TestAutofac()
     {
         return "Autofac使用成功";
     }
 }
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

该用户仓储继承IRepository.cs 和Repository.cs之后，它就拥有了该仓储接口下的所有接口。

**测试**

做完以上工作，我们开始测试

首先，我们需要创建数据库（使用sqlServer数据库）和用户表

![](https://img2024.cnblogs.com/blog/1158526/202410/1158526-20241006103536528-213382445.png)

表结构和数据代码如下：

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
USE [OverallAuth]
GO
/****** Object:  Table [dbo].[Sys_User]    Script Date: 2024/10/6 10:38:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sys_User](
    [UserId] [int] IDENTITY(1,1) NOT NULL,
    [UserName] [varchar](50) NOT NULL,
    [Password] [varchar](50) NOT NULL,
    [Age] [int] NULL,
    [Sex] [int] NULL,
    [DepartmentId] [int] NOT NULL,
    [IsOpen] [bit] NULL,
    [CreateTime] [datetime] NULL,
    [CreateUser] [varchar](50) NULL,
 CONSTRAINT [PK_Sys_User] PRIMARY KEY CLUSTERED 
(
    [UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Sys_User] ON 

INSERT [dbo].[Sys_User] ([UserId], [UserName], [Password], [Age], [Sex], [DepartmentId], [IsOpen], [CreateTime], [CreateUser]) VALUES (1, N'张三', N'1', 18, 1, 1, 1, CAST(N'2024-10-06T09:14:13.000' AS DateTime), N'1')
INSERT [dbo].[Sys_User] ([UserId], [UserName], [Password], [Age], [Sex], [DepartmentId], [IsOpen], [CreateTime], [CreateUser]) VALUES (2, N'李四', N'1', 19, 1, 1, 1, CAST(N'2024-10-06T09:15:08.000' AS DateTime), N'1')
SET IDENTITY_INSERT [dbo].[Sys_User] OFF
ALTER TABLE [dbo].[Sys_User] ADD  DEFAULT ((0)) FOR [IsOpen]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'UserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户密码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'Password'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户年龄' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'Age'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户年龄 1:男 2:女' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'Sex'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门id（表Department主键）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'DepartmentId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User', @level2type=N'COLUMN',@level2name=N'CreateUser'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'人员表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Sys_User'
GO
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

服务层编写接口调用

说明：以下这些类，都在上一期中有说道，这里不再多说。

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
/// <summary>
 /// 用户服务接口
 /// </summary>
 public interface ISysUserService
 {
     /// <summary>
     /// 测试Autofac
     /// </summary>
     /// <returns></returns>
     string TestAutofac();

     /// <summary>
     /// 查询所有用户
     /// </summary>
     /// <returns></returns>
     List<SysUser> GetAllUser();
 }
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
/// <summary>
/// 用户服务接口实现
/// </summary>
public class SysUserService : ISysUserService
{
    #region 构造实例化

    private readonly ISysUserRepository _sysUserRepository;

    public SysUserService(ISysUserRepository sysUserRepository)
    {
        _sysUserRepository = sysUserRepository;
    }

    #endregion

    /// <summary>
    /// 测试Autofac
    /// </summary>
    /// <returns></returns>
    public string TestAutofac()
    {
        return _sysUserRepository.TestAutofac();
    }

    /// <summary>
    /// 查询所有用户
    /// </summary>
    /// <returns></returns>
    public List<SysUser> GetAllUser() 
    {
        return _sysUserRepository.GetAll(BaseSqlRepository.sysUser_selectAllSql);
    }
}
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

在控制器（SysUserController）中添加如下接口

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
  /// <summary>
   /// 查询所有用户
   /// </summary>
   /// <returns></returns>
   [HttpGet]
   public List<SysUser> GetAllUser() 
   {
       return _userService.GetAllUser();
   }
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

好了，启动项目，进行测试

![](https://img2024.cnblogs.com/blog/1158526/202410/1158526-20241006104809855-2134863052.png)

可以看到，数据获取成功，到这里，我们使用Dapper搭建底层仓储连接数据库成功。

**源代码地址：https://gitee.com/yangguangchenjie/overall-auth2.0-web-api**  

**预览地址：http://139.155.137.144:8880/swagger/index.html**

**帮我Star，谢谢。**

**有兴趣的朋友，请关注我微信公众号吧(\*^▽^\*)。**

**![](https://img2024.cnblogs.com/blog/1158526/202408/1158526-20240824140446786-404771438.png)**

关注我：一个全栈多端的宝藏博主，定时分享技术文章，不定时分享开源项目。关注我，带你认识不一样的程序世界
