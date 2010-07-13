using UnityEngine;
using System.Collections;

public class MatrixTest : UUnitTestCase {

	public void TestLinearPath ()
	{
		float[,] m = new float[4,4];
		m[0,0] = 1;
		m[1,0] = 1;
		m[2,0] = 1;
		m[3,0] = 1;
		m[0,1] = -1;
		m[1,1] = 0;
		m[2,1] = 1;
		m[3,1] = 2;
		m[0,2] = 1;
		m[1,2] = 0;
		m[2,2] = 1;
		m[3,2] = 4;
		m[0,3] = -1;
		m[1,3] = 0;
		m[2,3] = 1;
		m[3,3] = 8;
		Matrix4x4 copy = new Matrix4x4();
		for (int i=0;i<4;i++){
			for (int j=0;j<4;j++){
				copy[i,j] = m[i,j];
			}
		}
		bool res = Matrix.Invert(m);
		Assert.True(res,"Inserve matrix");
		
		Debug.Log("InverseMatrix: "+Matrix.ToString(m));
		
		float[,] v = new float[4,1];
		v[0,0] = 2;
		v[1,0] = 0;
		v[2,0] = -2;
		v[3,0] = 0;
		
		
		copy = Matrix4x4.Inverse(copy);
		Debug.Log("Should be: \n"+copy);
		for (int i=0;i<4;i++){
			for (int j=0;j<4;j++){
				Assert.Equals (copy[i,j], m[i,j],0.01f, "At "+i+" , "+j);
			}
		}
		
		
		float[,] resM = Matrix.Multiply(m,v);
		Debug.Log("Matrix multiplication "+Matrix.ToString(resM));
		
	}
	
	public void TestMatrixMult(){
		Matrix4x4 mUnity1 = new Matrix4x4();
		Matrix4x4 mUnity2 = new Matrix4x4();
		float[,] m1 = new float[4,4];
		float[,] m2 = new float[4,4];
		
		for (int i=0;i<4;i++){
			for (int j=0;j<4;j++){
				float r1 = Random.value;
				float r2 = Random.value;
				mUnity1[i,j] = r1;
				m1[i,j] = r1;
				mUnity2[i,j] = r2;
				m2[i,j] = r2;
			} 
		}
		Matrix4x4 mUnity3 = mUnity1*mUnity2;
		float[,] r3 = Matrix.Multiply(m1,m2);
		
		for (int i=0;i<4;i++){
			for (int j=0;j<4;j++){
				Assert.Equals(mUnity3[i,j], r3[i,j],0.001f, "At "+i+" , "+j);
			}
		}
		
	}
}
