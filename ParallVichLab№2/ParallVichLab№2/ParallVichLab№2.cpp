// ParallVichLab№2.cpp: определяет точку входа для консольного приложения.
//

#include "stdafx.h"
#include <iostream>
#include <fstream>
#include <vector>
#include <omp.h>
#include <ctime>
using namespace std;

double Determinant(int N, vector<vector<double>> M)
{
	omp_set_nested(1);
	int mix = 1;
	for (int i = 0; i < N - 1; ++i)
	{
		if (fabs(M[i][i]) < 1e-6)
		{
			int j;
			for (j = i + 1; j < N; ++j)
			{
				if (fabs(M[j][i]) > 1e-6)
				{
#pragma omp parallel for schedule(static)
					for (int k = i; k < N; ++k)
					{
						double tmp = M[i][k];
						M[i][k] = M[j][k];
						M[j][k] = tmp;
					}
					break;
				}
			}
			if (j == N)
				return 0;
			mix*=-1;
		}
#pragma omp parallel for schedule(static)
		for (int j = i + 1; j < N; ++j)
		{
			if (fabs(M[j][i]) > 1e-6)
			{
				double factor = M[j][i] / M[i][i];
#pragma omp parallel for schedule(static)
				for (int k = i + 1; k < N; ++k)
				{
					M[j][k] -= M[i][k] * factor;
				}
			}
		}
	}
	double det = mix;
#pragma omp parallel for reduction(*:det)
	for (int i = 0; i < N; ++i)
	{
		det *= M[i][i];
	}
	return det;
}

int main()
{
	srand(time(0));
	int N;
	int c, ind, num;
	ifstream fin("test.txt");
	fin >> N;
	vector <vector<double>> Matrix(N, vector<double>(N));
	for(int i = 0; i<N; i++){
		fin >> c;
		while (c != 0) {
			fin >> ind;
			fin >> num;
			Matrix[i][ind] = num;
			c--;
		}
	}
	double timein, timeout, timeres = 0;
	timein = omp_get_wtime();
	cout << "Determinant "<< Determinant(N, Matrix)<<endl;
	timeout = omp_get_wtime();
	cout <<  timeout - timein << endl;
	system("pause");
    return 0;
}

