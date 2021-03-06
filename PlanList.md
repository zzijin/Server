# 计划列表

## 功能新增计划列表
* `XL 临时、简单功能补充，链接类型转换，监听流程完善，以及最后的通信模块功能代码` 2022.2.19--2022.2.24
进度 [完成]

## 代码重构计划列表
* `M 正式链接存储改为线程安全字典` 
进度 [无]
* `S 正式链接消息队列改为线程安全队列` 2022.2.24
进度 [完成]
* `M 正式链接消息队列类型更改，不再使用MsgPack与MsgBody，而是使用新的类型表示待执行和发送的任务，该类型在转换时会自我验证`
进度 [无]
* `M 调整链接处理线程`
进度 [无]

## 代码问题排除列表
* `严重 大量链接接入时有链接掉出` 2022.2.26--
进度 [完成]
  `Test1 普通测试`
  测试结果 掉线严重(少量未发送消息;大多数发送完了消息但未接收到信息)、处理慢但CPU使用率不高
  推测 掉线率与链接涌入量和线程优先级有关、链接连接服务器由临时连接转正式时处理可能存在纰漏
  结论 需要继续测试
  `Test2 设置服务器进程优先级为中高级`
  测试结果 在不续发消息下不存在掉线、续发消息下掉线严重(全部发送完了消息但未接收到信息)、处理慢但CPU使用率不高
  推测 临时链接不能及时处理第一天消息
  结论 掉线原因不明
  `Test3 修改统计信息，将虚拟消息信息与实际消息信息分开以探查真的发送情况`

* `严重 链接断开后未进入空闲队列`
进度 [完成]
	交由线程池循环线程检查

* `严重 执行/发送线程可能不会停止`
进度 [完成]

* `严重 等待链接至正式链接不会正常执行`
进度 [完成]

* `一般 无法保证文件被唯一创建在缓存，多线程访问可能多次创建`
进度 [进行中]
  `不影响结果，但可能会浪费磁盘读写性能`