#include <stdlib.h>
#include <stdio.h>
#include <stdbool.h>

struct Circle{
	int Index;
	int Priority;
	int CircleType;
	bool Already;
	bool Value;
	int *Inputs;
	int *Outs;
};

void printCircles(int n, struct Circle *circles);
void detectPriority(int n, struct Circle *circles);

void printCircles(int n, struct Circle *circles){
	int i;
	for(i = 0; i < n; i++){		
		printf("INDEX:%d Priority:%d TYPE:%d Already:%d Value:%d\n",
			circles[i].Index, circles[i].Priority, circles[i].CircleType, circles[i].Already, circles[i].Value);
	}
}

void detectPriority(int n, struct Circle *circles){
	queue<int> = null;
	int i;
	do{
		for(i = 0; i < n, i++){
			if(circles[i].Already){continue;}
			//PI
			if(circles[i].CircleType == 0){circles[i].Priority = queue.dequeue();}
			else{
				
			}
		}

	}while(queue.count != 0);
}

int main(int argc, char *argv){
	FILE *fp;
	fp = fopen("ex5.tbl", "r");
	if(fp == NULL){printf("File not exists"); exit(-1);}

	int count;
	fscanf(fp, "%d", &count);	
	printf("%d\n", count);
	struct Circle *circles;
	circles = (struct Circle*)malloc(sizeof(struct Circle) * count);	

	//load circles
	int i, j;
	for(i = 0; i < count; i++){
		int type, input_count, input, out_count, out;
		fscanf(fp, "%d %d %d %d %d", &type, &input_count, &input, &out_count, &out);		
		circles[i].Index = i;
		circles[i].CircleType = type;
		circles[i].Priority = -1;
		circles[i].Value = false;
		circles[i].Already = false;
	}

	//load inputs list
	//skip
	fscanf(fp, "");
	int *inputs;
	int inputs_count;
	fscanf(fp, "%d", &inputs_count);
	inputs = (int*)malloc(sizeof(int) * inputs_count);
	for(i = 0; i < inputs_count; i++){
		int a;
		fscanf(fp, "%d", &a);
		inputs[i] = a;
	}
	
	//return head
	fseek(fp, 0, SEEK_SET);	
	fscanf(fp, "%d", &count);

	for(i = 0; i < count; i++){
		int type, input_count, input, out_count, out;
		fscanf(fp, "%d %d %d %d %d", &type, &input_count, &input, &out_count, &out);

		int *inarray;
		inarray = (int*)malloc(sizeof(int) * input_count);
		if(input_count > 1){
			int start = input;
			for(j = 0; j < input_count; j++){
				inarray[j] = inputs[start + j];
			}
		}else{
			inarray[0] = input;
		}

		int *outarray;
		outarray = (int*)malloc(sizeof(int) * out_count);
		if(out_count > 1){
			int start = out;
			for(j = 0; j < out_count; j++){
				outarray[j] = inputs[start + j];
			}
		}else{
			outarray[0] = out;
		}

		circles[i].Inputs = inarray;
		circles[i].Outs = outarray;
	}

	printCircles(count, circles);

	free(circles);
	fclose(fp);	

	return 0;
}
