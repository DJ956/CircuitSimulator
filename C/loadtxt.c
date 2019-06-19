#include <stdlib.h>
#include <stdio.h>

void loadCircle(char *fileName, char *output);
void loadFaults(char *fileName, char *output);

void loadCircle(char *fileName, char *output){
	int i;
	FILE *fi = fopen(fileName, "r");
	FILE *fo = fopen(output, "w");

	if(fi == NULL){printf("load input failed \n"); exit(-1);}
	if(fo == NULL){printf("load output failed\n"); exit(-1);}
	//list 1
	int count;

	fscanf(fi, "%d", &count);
	fprintf(fo, "%d\n", count);
	for(i = 0; i < count; i++){
		int type, inputCount, input, outCount, out;
		fscanf(fi, "%d %d %d %d %d", &type, &inputCount, &input, &outCount, &out);
		fprintf(fo, "%d,%d,%d,%d,%d\n", type, inputCount, input, outCount, out);
	}
	fprintf(fo, "\n");
	//list 2
	fscanf(fi, "%d", &count);
	fprintf(fo, "%d\n", count);
	for(i = 0; i < count; i++){
		int index;
		fscanf(fi, "%d", &index);
		fprintf(fo, "%d\n", index);
	}
	fprintf(fo, "\n");

	//outside inputs
	fscanf(fi, "%d", &count);
	fprintf(fo, "%d\n", count);
	for(i = 0; i < count; i++){
		int index;
		fscanf(fi, "%d", &index);
		fprintf(fo, "%d\n", index);
	}
	fprintf(fo, "\n");

	//outside outputs
	fscanf(fi, "%d", &count);
	fprintf(fo, "%d\n", count);
	for(i = 0; i < count; i++){
		int index;
		fscanf(fi, "%d", &index);
		fprintf(fo, "%d\n", index);
	}

	fclose(fi);
	fclose(fo);	
}

void loadFaults(char *fileName, char *output){
	int i;
	FILE *fi = fopen(fileName, "r");
	FILE *fo = fopen(output, "w");

	if(fi == NULL){printf("load input failed \n"); exit(-1);}
	if(fo == NULL){printf("load outputs failed \n"); exit(-1);}
	//frep
	int count;

	fscanf(fi, "%d", &count);
	fprintf(fo, "%d\n", count);
	for(i = 0; i < count; i++){
		int faultIndex, faultValue;
		fscanf(fi, "%d %d", &faultIndex, &faultValue);
		fprintf(fo, "%d,%d\n", faultIndex, faultValue);
	}
}

//argv[1] = inputFileName, argv[2]=outputFileName, argv[3] = load type
//type 1 = tbl, type 2 = faults
int main(int argc, char *argv[]){
	if(argc != 4){
		printf("arguments exception\n");
		exit(-1);
	}
	
	int type;
	type = atoi(argv[3]);

	switch(type){
		case 1:{
			printf("load circle\n");
			loadCircle(argv[1], argv[2]);
			break;
		}
		case 2:{
			printf("load faults\n");
			loadFaults(argv[1], argv[2]);
			break;
		}default:{
			printf("error\n");
			break;
		}
	}
	printf("save reformat\n");

	return 0;
}