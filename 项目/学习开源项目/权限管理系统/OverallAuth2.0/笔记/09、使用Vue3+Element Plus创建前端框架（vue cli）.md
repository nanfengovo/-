---
created: 2024-12-16T22:17:41 (UTC +08:00)
tags: [从0到1 搭建OverallAuth2.0 权限管理系统,OverallAuth2.0 权限管理系统]
source: https://www.cnblogs.com/cyzf/p/18496583
author: 陈逸子风
---

# （系列九）使用Vue3+Element Plus创建前端框架（附源码） - 陈逸子风 - 博客园

> ## Excerpt
> 说明 该文章是属于OverallAuth2.0系列文章，每周更新一篇该系列文章（从0到1完成系统开发）。 该系统文章，我会尽量说的非常详细，做到不管新手、老手都能看懂。 说明：OverallAuth2.0 是一个简单、易懂、功能强大的权限+可视化流程管理系统。 友情提醒：本篇文章是属于系列文章，看该

---
**说明  
**

    该文章是属于OverallAuth2.0系列文章，每周更新一篇该系列文章（从0到1完成系统开发）。

    该系统文章，我会尽量说的非常详细，做到不管新手、老手都能看懂。

    说明：OverallAuth2.0 是一个简单、易懂、功能强大的权限+可视化流程管理系统。

友情提醒：本篇文章是属于系列文章，看该文章前，建议先看之前文章，可以更好理解项目结构。

qq群：801913255

**有兴趣的朋友，请关注我吧(\*^▽^\*)。**

**![](https://img2024.cnblogs.com/blog/1158526/202408/1158526-20240824140446786-404771438.png)**

**关注我，学不会你来打我**

**废话文学**

震惊！什么？你还不会使用Vue3+Element Plus搭建前端框架？

什么？你还在为找Vue3+Element Plus的前端框架模板而烦恼？

简单的不符合心意，成熟的又复杂看不懂？怎么办？

那还等什么，关注我，手把手教你搭建自己的前端模板。

**创建项目，前置条件**

安装VsCode

安装脚手架：npm install -g @vue/cli  我的版本v5.0.8

安装node.js（下载地址）：[点击下载](https://nodejs.cn/en/download)  我用的版本v21.7.3

**创建Vue3项目**

打开vsCode，在终端中切换创建项目路径（如果不切换，默认安装在C:\\Users\\admin）：如：cd  E:\\Vue项目

使用vue create xxx命令创建项目，这里需要注意的是，项目名称中，不能包含大写字母

如下图所示

![](https://img2024.cnblogs.com/blog/1158526/202410/1158526-20241023163016648-1752485650.png)

这里我们直接选择：使用Vue3安装，当然你也可以选择手动选择功能。

安装成功后，我们打开项目。

![](https://img2024.cnblogs.com/blog/1158526/202410/1158526-20241023163749184-2039205251.png)

可以看到，新建项目的目录结构，非常简单明了。我们使用命令，运行看下效果

运行命令：npm run serve

![](https://img2024.cnblogs.com/blog/1158526/202410/1158526-20241023163954108-1098494260.png)

默认项目样式

![](https://img2024.cnblogs.com/blog/1158526/202410/1158526-20241023164132962-1561261847.png)

出现如下界面，证明我们创建项目成功。

**安装Element Plus**

\--使用npm

npm install element-plus --save 

\--使用

yarn add element-plus

\--使用

pnpm install element-plus

安装成功后，我们在main中配置全局变量

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
import { createApp } from 'vue'
import App from './App.vue'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'

const app = createApp(App)
app.use(ElementPlus)
app.mount('#app')
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

 安装小图标：npm install @element-plus/icons-vue   

全局配置：

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
import { createApp } from 'vue'
import App from './App.vue'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import * as Icons from '@element-plus/icons-vue'

const app = createApp(App)
app.use(ElementPlus)
for (const [key, component] of Object.entries(Icons)) {
    app.component(key, component)
  }
app.mount('#app')
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

安装Typescript :npm install  --save-dev typescript ts-loader

把js文件转成ts： vue add typescript

说明下：因为element plus 官方用例也是使用ts，所以我们需要把js转换成ts

添加tsconfig.json配置文件，示例如下（不然使用ts会报错）

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
{
  "compilerOptions": {
    "target": "esnext",
    "module": "esnext",
    "strict": true,
    "jsx": "preserve",
    "moduleResolution": "node",
    "experimentalDecorators": true,
    "allowJs": true,
    "skipLibCheck": true,
    "esModuleInterop": true,
    "allowSyntheticDefaultImports": true,
    "forceConsistentCasingInFileNames": true,
    "useDefineForClassFields": true,
    "sourceMap": true,
    "baseUrl": ".",
    "types": [
      "webpack-env"
    ],
    "paths": {
      "@/*": [
        "src/*"
      ]
    },
    "lib": [
      "esnext",
      "dom",
      "dom.iterable",
      "scripthost"
    ]
  },
  "include": [
    "src/**/*.ts",
    "src/**/*.tsx",
    "src/**/*.vue",
    "tests/**/*.ts",
    "tests/**/*.tsx"
  ],
  "exclude": [
    "node_modules"
  ]
}
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

做完以上操作后，我们vue项目的基本结构如下图

![](https://img2024.cnblogs.com/blog/1158526/202410/1158526-20241024140353324-1119172645.png)

**测试Element Plus**

做好以上步骤，我们vue3+Element Plus搭建项目的基本模板已经创建好，接下来测试Element Plus是否可以使用

这里我选择使用Element Plus 中的Menu组件，因为接下来我们会使用Menu做系统菜单

找到系统默认生成的HelloWorld.vue文件，用如下代码替换里面的代码

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
<template>
      <el-menu
        active-text-color="#ffd04b"
        background-color="#545c64"
        class="el-menu-vertical-demo"
        default-active="2"
        text-color="#fff"
      >
        <el-sub-menu index="1">
          <template #title>
            <el-icon><location /></el-icon>
            <span>Navigator One</span>
          </template>
          <el-menu-item-group title="Group One">
            <el-menu-item index="1-1">item one</el-menu-item>
            <el-menu-item index="1-2">item two</el-menu-item>
          </el-menu-item-group>
          <el-menu-item-group title="Group Two">
            <el-menu-item index="1-3">item three</el-menu-item>
          </el-menu-item-group>
          <el-sub-menu index="1-4">
            <template #title>item four</template>
            <el-menu-item index="1-4-1">item one</el-menu-item>
          </el-sub-menu>
        </el-sub-menu>
        <el-menu-item index="2">
          <el-icon><icon-menu /></el-icon>
          <span>Navigator Two</span>
        </el-menu-item>
        <el-menu-item index="3" disabled>
          <el-icon><document /></el-icon>
          <span>Navigator Three</span>
        </el-menu-item>
        <el-menu-item index="4">
          <el-icon><setting /></el-icon>
          <span>Navigator Four</span>
        </el-menu-item>
      </el-menu>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  setup() {
  },
  components: {},
});
</script>
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

该代码，基本和官网的示例一致。是构建一个Menu导航菜单

然后再找到App.vue文件，用如下代码替换里面代码

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
<template>
  <HelloWorld />
</template>

<script lang="ts">
import { defineComponent } from "vue";
import HelloWorld from "./components/HelloWorld.vue"

export default defineComponent({
  setup() {
  },
  components: {HelloWorld},
});
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

 做好以上步骤后，我们使用npm run serve命令，启动项目

出下如下界面，证明项目创建成功

![](https://img2024.cnblogs.com/blog/1158526/202410/1158526-20241024141325317-1343124996.png)

**搭建框架**

安装好Element Plus后，我们就要使用它来搭建框架

使用Element Plus的布局组件container+菜单组件Menu，来搭建框架。

这里是写样式布局，没有啥好说的，直接上代码

HelloWorld.vue 代码如下

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
<template>
  <div style="height: calc(100vh); overflow: hidden">
    <el-container style="height: 100%; overflow: hidden">
      <el-aside width="auto">
        <el-menu
          class="el-menu-vertical-demo"
          background-color="#545c64"
          text-color="#fff"
          active-text-color="#ffd04b"
          style="height: 100%"
        >
        <div class="el-menu-box">
            <div
              class="logo-image"
              style="width: 18px; height: 18px; background-size: 18px 18px"
            ></div>
            <div style="padding-left: 5px; padding-top: 7px">
              OverallAuth2.0
            </div>
          </div>
          <el-sub-menu index="1">
            <template #title>
              <el-icon><location /></el-icon>
              <span>Navigator One</span>
            </template>
            <el-menu-item-group title="Group One">
              <el-menu-item index="1-1">item one</el-menu-item>
              <el-menu-item index="1-2">item two</el-menu-item>
            </el-menu-item-group>
            <el-menu-item-group title="Group Two">
              <el-menu-item index="1-3">item three</el-menu-item>
            </el-menu-item-group>
            <el-sub-menu index="1-4">
              <template #title>item four</template>
              <el-menu-item index="1-4-1">item one</el-menu-item>
            </el-sub-menu>
          </el-sub-menu>
          <el-menu-item index="2">
            <el-icon><icon-menu /></el-icon>
            <span>Navigator Two</span>
          </el-menu-item>
          <el-menu-item index="3" disabled>
            <el-icon><document /></el-icon>
            <span>Navigator Three</span>
          </el-menu-item>
          <el-menu-item index="4">
            <el-icon><setting /></el-icon>
            <span>Navigator Four</span>
          </el-menu-item>
        </el-menu>
      </el-aside>

      <el-container>
        <el-header class="headerCss">
          <div style="display: flex; height: 100%; align-items: center">
            <div
              style="
                text-align: left;
                width: 50%;
                font-size: 18px;
                display: flex;
              "
            >
              <div class="logo-image" style="width: 32px; height: 32px"></div>
              <div style="padding-left: 10px; padding-top: 7px">
                OverallAuth2.0 权限管理系统
              </div>
            </div>
            <div
              style="
                text-align: right;
                width: 50%;
                display: flex;
                justify-content: right;
                cursor: pointer;
              "
            >
              <div
                class="user-image"
                style="width: 22px; height: 22px; background-size: 22px 22px"
              ></div>
              <div style="padding-left: 5px; padding-top: 3px">王小虎</div>
            </div>
          </div>
        </el-header>

        <el-main class="el-main">
          欢迎
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  setup() {},
  components: {},
});
</script>

<style scoped>
.el-menu-vertical-demo:not(.el-menu--collapse) {
  width: 200px;
  min-height: 400px;
}
.el-menu-box {
  display: flex;
  padding-left: 25px;
  align-items: center;
  height: 57px;
  box-shadow: 0 1px 4px #00152914;
  border: 1px solid #00152914;
  color: white;
}
.el-main {
  padding-top: 0px;
  padding-left: 1px;
  padding-right: 1px;
  margin: 0;
}
.headerCss {
  font-size: 12px;
  border: 1px solid #00152914;
  box-shadow: 0 1px 4px #00152914;
  justify-content: right;
  align-items: center;
  /* display: flex; */
}
.logo-image {
  background-image: url("../components/权限分配.png");
}
.user-image {
  background-image: url("../components/用户.png");
}
.demo-tabs /deep/ .el-tabs__header {
  color: #333; /* 标签页头部字体颜色 */
  margin: 0 0 5px !important;
}
.demo-tabs /deep/ .el-tabs__nav-wrap {
  padding-left: 10px;
}
</style>
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

App.vue 代码如下

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

```
<template>
  <HelloWorld />
</template>

<script lang="ts">
import { defineComponent } from "vue";
import HelloWorld from "./components/HelloWorld.vue"

export default defineComponent({
  setup() {
  },
  components: {HelloWorld},
});
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  /* margin-top: 60px; */
}
html,  
    body,  
    #app {  
        height: 100%;  
        margin: 0;  
        padding: 0;  
    }
</style>
```

![复制代码](https://assets.cnblogs.com/images/copycode.gif)

ps：页面中的图标，可以用任何小图标替换（也可以删除）。

运行项目

![](https://img2024.cnblogs.com/blog/1158526/202410/1158526-20241024145609663-501656877.png)

 **下一篇：Vue3菜单和路由的结合使用**

****后端WebApi** 预览地址：http://139.155.137.144:8880/swagger/index.html**

**前端vue 预览地址：http://139.155.137.144:8881**

**关注公众号：发送【权限】，获取前后端代码**

**有兴趣的朋友，请关注我微信公众号吧(\*^▽^\*)。**

**![](https://img2024.cnblogs.com/blog/1158526/202408/1158526-20240824140446786-404771438.png)**

关注我：一个全栈多端的宝藏博主，定时分享技术文章，不定时分享开源项目。关注我，带你认识不一样的程序世界
