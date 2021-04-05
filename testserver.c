#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <unistd.h>
#include <io.h>
#include <fcntl.h>
#include <share.h>
#include <conio.h> 
#include <unistd.h>
#define SIZE 512

void write_file(int sockfd){
  //Read Picture Byte Array and Copy in file
  printf("Reading Picture Byte Array\n");
  FILE *image = fopen("hanna2.jpeg", "wb");
  char p_array[SIZE];
  int nb;
   do {
        nb = recv(sockfd, p_array, SIZE, 0);
        if ( nb > 0 ){
          printf("Bytes received: %d\n", nb);
          printf("size: %d\n", sizeof(p_array));
          fwrite(p_array, 1, nb, image);
        }
            
        else if ( nb == 0 ){
            printf("Connection closed\n");
        }
        else
            printf("recv failed: %d\n", WSAGetLastError());

    } while( nb > 0 );
    fclose(image);
  return;
}


int main(){
#if defined(_WIN32) || defined(_WIN64)    
    WSADATA wsa;
    if (WSAStartup(MAKEWORD(2,2),&wsa) != 0) {
        printf("\nError: Windows socket subsytsem could not be initialized. Error Code: %d. Exiting..\n", WSAGetLastError());
        exit(1);
    }
#endif


  char *ip = "130.242.97.224";
  int port = 8080;
  int e;

  int sockfd, new_sock;
  struct sockaddr_in server_addr, new_addr;
  socklen_t addr_size;
  char buffer[SIZE];



  sockfd = socket(AF_INET, SOCK_STREAM, 0);
  if(sockfd < 0) {
    perror("[-]Error in socket");
    exit(1);
  }
  printf("[+]Server socket created successfully.\n");

  

  server_addr.sin_family = AF_INET;
  server_addr.sin_port = port;
  server_addr.sin_addr.s_addr = inet_addr(ip);

  e = bind(sockfd, (struct sockaddr*)&server_addr, sizeof(server_addr));
  if(e < 0) {
    perror("[-]Error in bind");
    exit(1);
  }
  printf("[+]Binding successfull.\n");

  if(listen(sockfd, 10000) == 0){
		printf("[+]Listening....\n");
	}else{
		perror("[-]Error in listening");
    exit(1);
	}

  addr_size = sizeof(new_addr);
  new_sock = accept(sockfd, (struct sockaddr*)&new_addr, &addr_size);
  write_file(new_sock);
  
  printf("[+]Data written in the file successfully.\n");
  return 0;
}