# Server
## ����������
����ԭ��˼Ӧ������˰��ղ�ͬ���ܻ���Ϊ�������(�����֪�ͻ�����������˵ĵ�ַ���ͻ��˵�ע���¼��֤)���ļ������(����ͼƬ�����֡���Ƶ���ļ������´���)�����ݿ�����(����̬���ݺͶ�̬���۵���ɾ���)����������(�������칦��)����Ӧ���赽��ͬ���������Դﵽ���ؾ��⡣����ʱ��ɱ��ȹ�ϵ���ǽ����й��ܼ��ɵ�һ������ˡ�  
**���������(Sever)���������ģ�飺**  
>1.ͨѶģ�飺���������ά�ֿͻ��˵�����(�����¿ͻ��ˡ����ս��������Ӧ�ͻ�����Ϣ������ͻ�������)  
>2.���ݿ�ģ�飺�������ݿ����ɾ��Ĳ������Ż��������ֱ�פ�����ڴ���  
>3.�ļ�ģ�飺�����ļ��Ķ�ȡ��д�룬�Ż����������ļ�פ�����ڴ���  
>4.��־ģ�飺�����¼ҵ���ܲ�������־��Ϣ���ṩ��־�������ܣ���̬  
>5.ͳ��ģ�飺����ͳ��ҵ���ܸ�ģ���������Ϣ(�磺ͨѶģ�鵥�����ӻ��������Ӳ�����������/��Ϣ����/��Ϣ���ӳ�)�Թ�����ͳ�ƣ���̬  
* �������Ŀ���Ȱ���  
>[������־](https://github.com/zzijin/Server/blob/master/UpdateLog.md "������־")  
>1.��ɻ������ܺ���ɹ��ܲ���  
>2.����ҵ����룬ʵ����Ŀ��ҵ����  
>3.�Ż���ĿЧ�ʣ� ʹ�ø��ø����ʵ��㷨�����ݽṹ  
* ͨѶģ��  
	* ������ϸ����(��������������):  
		* 1.ʹ��SocketAsyncEventArgs��ʵ�ָ����ܵ���������� <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="���"/><br/>
			* ��.SocketAsyncEventArgs���װ��IOCP,ʹ��SocketAsyncEventArgs�����ͬ���ͱ����ڴ������첽�׽���I/O�ڼ��ظ������ͬ������  
		* 2.ʹ��Conn��(�������)��װsocket <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="���"/><br/>
			* ��.һ��Conn�����һ���ɹ����ӵ����ӻ������ӵ�����  
			* ��.��Conn��ʹ��ʱ���ṩ�����߳�(�����̳߳ع���)���ֱ�������ɽ��ս����������Ͳ���;�����ӶϿ�ʱConn�������Լ����ȴ��´�ʹ��  
		* 3.ʹ��ConnPool(������ӳ�)ͳһ��������Conn(�ɲ��ո��¼�¼��һ�μ�¼) <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="���"/><br/>
			* ��.ʹ��Conn[]����洢����Conn��ʹ���̰߳�ȫ���д洢����δʹ�õ�Conn�ĵ�ַ��ʹ�ô���������洢������ʹ�õ�Conn�ĵ�ַ  
			* ��.�����µ�Socket����ʱ����δʹ�ö����ײ���ȡ��һ��Conn������ʹ�������У����Դ�Conn��ֵ(Conn�������ڲ��������߳̿�ʼ������Ϣ)  
			* ��.��һ��Conn�ر�����ʱ�����ô�Conn���������ʹ��������ȡ��������δʹ�ö���ĩβ  
		* 4.ConnPool������Connʹ��ͳһ�Ļ�����(�ɲ��ո��¼�¼�ڶ��μ�¼) <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="���"/><br/>
			* ��.ConnPool���лᴴ��һ���ɹ�����Conn�շ�ʹ�õ�byte[]���飬���ڳ�ʼ�����ӳ�ʱ����ַ�ʹ�С���������Conn  
			* ��.ÿ��Connӵ�еĻ���������ConnBufferManager�����  
				* ��.Conn���ڳ�ʼ��ʱ����õĻ���������ʼλ�õȲ������ݸ�ConnBufferManager��  
				* ��.ConnBufferManager��ʹ��BufferIndex��ֱ�洢����/���ͻ������Ŀ�ʼλ�á�����λ�á�д����ʼλ�á���ȡ��ʼλ�á����д�С��ʹ�ô�С����Ϣ  
				* ��.��Conn���н��շ��Ͳ���ʱ��ConnBufferManager���ṩ�����ֱ��������ý������ͻ�����  
				* ��.��Conn���н�����д��ʱ��ConnBufferManager���ṩ��������ReadOnlySpan<byte>��Span<byte>�������ڶ�ȡ��д��  
				* ��.Conn��ʹ��ʱ�ὫConnBufferManager���ʼ��(�������ý���/���ͻ�������д����ʼλ�á���ȡ��ʼλ�á����д�С��ʹ�ô�С����Ϣ)  
		* 5.Connʹ�ö��д洢��Ϣ(�ɲ��ո��¼�¼�����μ�¼) <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="���"/><br/>
			* ��.ʹ�ö��д洢��Ϣ���ν�Conn�Ľ��ս����̡߳��������̺߳������ⲿ�߳�(����ConnPool�����̶߳��ڷ���������Ϣ����ͬConn��ͨ�š��ȵ�)  
			* ��.ʹ�ö��д洢��Ϣһ���̶����ܹ�����������ã�ʹ���������ȶ�  
			* ��.Connʹ��ConnMsgQueue�����ȴ�������Ϣ���к͵ȴ�������Ϣ����  
		* 6.Conn������Ϣʱʹ���ֵ������Ӧ���� <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="���"/><br/>
			* ��.����¼  
		* 7.ʹ��ConnInfo��¼Conn��Ϣ��ʹ��ConnPoolInfo��¼ConnPool��Ϣ <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="���"/><br/>
			* ��.����¼  
		* 8.ʹ��TemporaryConn��װ�¼������Ļ�δ�������ӳص�socket <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="���"/><br/>
			* ��.����¼  
		* 9.����ͨ�ù̶����ͻ���������Ԥ�洢����Ϣ <img src="https://github.com/zzijin/Server/blob/master/ImageResoure/realize.png" width="auto" height="auto" alt="���"/><br/>
			* ��.����¼  
		* 10.ʹ��ConnToken��ʵ�ֶ�����������(�����ƣ�����Ϊ��������)  
			* ��.��ע���˶����������ܵ�Conn����ʵ�ֶ�����������(ע�ἴ����ConnToken��)  
				* ��.ʹ�ÿͻ��˷���Ψһ��ʶ(��������ͻ���MAC��ȵ�)����ConnToken��  
				* ��.ConnToken������Ψһ��ʶͨ����ϣ�㷨������Կ�����ݻؿͻ���  
			* ��.��socket����Ͽ�ʱ��ӵ��ConnToken��Conn����ֱ��ͣ�ò����÷Żؿ������Ӷ��У����ǻὫConnToken���е�connConnectState��Ϊfalse����¼�Ͽ�ʱ��  
			* ��.ConnPool��������߳���ѭ������������ϢʱҲ����Conn��ConnToken�࣬��ConnToken���е�connConnectState=false���ҶϿ�ʱ������趨�Ͽ�ʱ��ʱ���϶�������ʧЧ�����ò�����������Ӷ���  
			* ��.���ͻ�������ʱ�����ݿͻ��˷��͵�Ψһ��ʶ����Կ��ѯ��Ӧ��Conn���������  
		* 11.������ʱ�������������������ӳ������������ӽ�������ʱ������(���������շ�����)���ȴ��Ŷӵȴ�����������Ϊ�����������Զ�������(Ԥ�蹦�ܣ�����δʵ��)  