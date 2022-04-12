# Server
## 服务端总设计
按照原构思应将服务端按照不同功能划分为主服务端(负责告知客户端其它服务端的地址；客户端的注册登录验证)、文件服务端(负责图片、音乐、视频等文件的上下传输)、数据库服务端(负责动态内容和动态评论的增删查改)、聊天服务端(负责聊天功能)并且应架设到不同服务器上以达到负载均衡。由于时间成本等关系于是将所有功能集成到一个服务端。  
**整个服务端(Sever)将包含多个模块：**  
>1.通讯模块：负责监听与维持客户端的连接(接入新客户端、接收解析处理回应客户端消息、管理客户端链接)  
>2.数据库模块：负责数据库的增删查改操作，优化：将部分表驻留在内存中  
>3.文件模块：负责文件的读取和写入，优化：将部分文件驻留在内存中  
>4.日志模块：负责记录业务功能产生的日志信息并提供日志分析功能，静态  
>5.统计模块：负责统计业务功能各模块产生的信息(如：通讯模块单个链接或所有链接产生的数据量/消息包量/消息包延迟)以供分析统计，静态  
* 服务端项目进度安排  
>[更新日志](https://github.com/zzijin/Server/blob/master/UpdateLog.md "更新日志")  
>1.完成基本功能和完成功能测试  
>2.增添业务代码，实现项目的业务功能  
>3.优化项目效率， 使用更好更合适的算法和数据结构  
* 通讯模块  
	* 代码详细介绍(跟随完成情况更新):  
		* 1.使用SocketAsyncEventArgs类实现高性能的网络服务器 <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="完成"/><br/>
			* ①.SocketAsyncEventArgs类封装了IOCP,使用SocketAsyncEventArgs类替代同步和避免在大容量异步套接字I/O期间重复分配和同步对象  
		* 2.使用Conn类(简称链接)封装socket <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="完成"/><br/>
			* ①.一个Conn类代表一个可供连接的链接或已连接的链接  
			* ②.当Conn被使用时会提供两个线程(均有线程池管理)，分别用于完成接收解析、处理发送操作;当连接断开时Conn会重置自己并等待下次使用  
		* 3.使用ConnPool(简称链接池)统一管理所有Conn(可参照更新记录第一次记录) <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="完成"/><br/>
			* ①.使用Conn[]数组存储所有Conn，使用线程安全队列存储所有未使用的Conn的地址，使用带锁的链表存储所有已使用的Conn的地址  
			* ②.当有新的Socket接入时，从未使用队列首部中取出一个Conn加入已使用链表中，并对此Conn赋值(Conn会启动内部的两个线程开始处理信息)  
			* ③.当一个Conn关闭连接时，重置此Conn并将其从已使用链表中取出并加入未使用队列末尾  
		* 4.ConnPool中所有Conn使用统一的缓冲区(可参照更新记录第二次记录) <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="完成"/><br/>
			* ①.ConnPool类中会创建一个可供所有Conn收发使用的byte[]数组，并在初始化链接池时按地址和大小分配给所有Conn  
			* ②.每个Conn拥有的缓冲区交由ConnBufferManager类管理  
				* Ⅰ.Conn类在初始化时将获得的缓冲区、起始位置等参数传递给ConnBufferManager类  
				* Ⅱ.ConnBufferManager类使用BufferIndex类分别存储接收/发送缓冲区的开始位置、结束位置、写入起始位置、读取起始位置、空闲大小、使用大小等信息  
				* Ⅲ.在Conn进行接收发送操作时，ConnBufferManager类提供方法分别用于设置结束发送缓冲区  
				* Ⅳ.在Conn进行解析和写入时，ConnBufferManager类提供方法返回ReadOnlySpan<byte>和Span<byte>对象用于读取和写入  
				* Ⅴ.Conn不使用时会将ConnBufferManager类初始化(重新设置接收/发送缓冲区的写入起始位置、读取起始位置、空闲大小、使用大小等信息)  
		* 5.Conn使用队列存储消息(可参照更新记录第三次记录) <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="完成"/><br/>
			* ①.使用队列存储消息以衔接Conn的接收解析线程、处理发送线程和其它外部线程(例如ConnPool心跳线程定期发送心跳消息、不同Conn间通信、等等)  
			* ②.使用队列存储消息一定程度上能够起到削峰等作用，使服务器更稳定  
			* ③.Conn使用ConnMsgQueue类管理等待处理消息队列和等待发送消息队列  
		* 6.Conn处理消息时使用字典完成相应操作 <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="完成"/><br/>
			* ①.待补录  
		* 7.使用ConnInfo记录Conn信息，使用ConnPoolInfo记录ConnPool信息 <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="完成"/><br/>
			* ①.待补录  
		* 8.使用TemporaryConn封装新监听到的还未进入链接池的socket <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="完成"/><br/>
			* ①.待补录  
		* 9.设立通用固定发送缓冲区发送预存储的信息 <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="完成"/><br/>
			* ①.待补录  
		* 10.使用ConnToken类实现断线重连功能(待完善，以下为初步流程)  
			* ①.已注册了断线重连功能的Conn才能实现断线重连功能(注册即创建ConnToken类)  
				* Ⅰ.使用客户端发送唯一标识(待定，如客户端MAC码等等)生成ConnToken类  
				* Ⅱ.ConnToken类会根据唯一标识通过哈希算法生成密钥并传递回客户端  
			* ②.当socket意外断开时，拥有ConnToken的Conn不会直接停用并重置放回空闲链接队列，而是会将ConnToken类中的connConnectState设为false并记录断开时间  
			* ③.ConnPool类的心跳线程在循环发送心跳信息时也会检查Conn的ConnToken类，若ConnToken类中的connConnectState=false并且断开时间大于设定断开时间时，认定此链接失效，重置并放如空闲链接队列  
			* ④.当客户端重连时，根据客户端发送的唯一标识和密钥查询相应的Conn并完成重连  
		* 11.划分临时链接区，若服务器链接池已满，新链接将加入临时链接区(限制链接收发流量)并等待排队等待，若新连接为重连链接则尝试断线重连(预设功能，代码未实现)  