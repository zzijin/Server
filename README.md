# Server
## 服务端总设计
按照原构思应将服务端按照不同功能划分为主服务端(负责告知客户端其它服务端的地址；客户端的注册登录验证)、文件服务端(负责图片、音乐、视频等文件的上下传输)、数据库服务端(负责动态内容和动态评论的增删查改)、聊天服务端(负责聊天功能)并且应架设到不同服务器上以达到负载均衡。由于时间成本等关系于是将所有功能集成到一个服务端。  
**整个服务端(Sever)将包含多个模块：**  
>1.通讯模块：负责维护与管理客户端的TCP连接  
>2.数据库模块：负责数据库的增删查改操作  
>3.文件模块：负责文件的基于内存缓存的读取和写入  
>4.日志模块：负责记录业务功能产生的日志信息并提供日志分析功能  
>5.统计模块：负责统计业务功能各模块产生的信息(如：通讯模块单个链接或所有链接产生的数据量/消息包量/消息包延迟)以供分析统计  
>[更新日志](https://github.com/zzijin/Server/blob/master/UpdateLog.md "更新日志")  
  
! 由于git数据意外丢失，提交记录已被清除  
! 项目已从.NET Framework迁移至.NET Core  
! 新的技术(参照了ASP.NET的技术体系)：使用DI服务容器、使用内存缓存、使用EF Core...  
  
**进行中的开发**  
>1.测试完成服务端目前的文件读写功能  
>2.完善服务端目前的所有功能，如日志输出、数据输出等  

**未来的开发**  
>1.通信安全性：私密数据将使用非对称加密方式加密传输，非私密数据将使用摘要算法防止数据被篡改(参照JWT)  
>2.代码设计质量：将项目向框架化发展，总体架构上可以参考DDD设计  