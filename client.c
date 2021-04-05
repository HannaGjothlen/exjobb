#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <arpa/inet.h>
#define SIZE 512

void send_file(FILE *fp, int sockfd){
  int n;
  char data[SIZE];

  while(fgets(data, SIZE, fp) != NULL) {
    if (send(sockfd, data, sizeof(data), 0) == -1) {
      perror("[-]Error in sending file.");
      exit(1);
    }
    bzero(data, SIZE);
  }

}

int main(){
  char *ip = "130.242.97.224";
  //char *ip = "127.0.0.1";
  int port = 8080;
  int e;

  int sockfd;
  struct sockaddr_in server_addr;
  FILE *fp;

  sockfd = socket(AF_INET, SOCK_STREAM, 0);
  if(sockfd < 0) {
    perror("[-]Error in socket");
    exit(1);
  }
  printf("[+]Server socket created successfully.\n");

  server_addr.sin_family = AF_INET;
  server_addr.sin_port = port;
  server_addr.sin_addr.s_addr = inet_addr(ip);

  e = connect(sockfd, (struct sockaddr*)&server_addr, sizeof(server_addr));
  if(e == -1) {
    perror("[-]Error in socket");
    exit(1);
  }
	printf("[+]Connected to Server.\n");


  FILE *picture = fopen("salem.jpeg", "rb");
  char send_buffer[SIZE];
  int nb = fread(send_buffer, 1, sizeof(send_buffer), picture);
  while(!feof(picture)) {
    write(sockfd, send_buffer, nb);
    nb = fread(send_buffer, 1, sizeof(send_buffer), picture);
  }
  send_file(picture, sockfd);

  printf("[+]File data sent successfully.\n");

	printf("[+]Closing the connection.\n");
  close(sockfd);

  return 0;
}
