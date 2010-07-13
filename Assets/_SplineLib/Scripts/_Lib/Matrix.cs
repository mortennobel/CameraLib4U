using UnityEngine;
using System.Collections;

/**
 * matrix[row,column]
 **/
public static class Matrix {
	public static string ToString(float[,] m){
		string s = "\n";
		for (int i=0;i<m.GetLength(0);i++){
			for (int j=0;j<m.GetLength(1);j++){
				s+=string.Format("{0,20:n} ", m[i,j]);
			}
			s+="\n";
		}
		return s;
	}
		
	
	public static float[,] Multiply(float[,] a, float[,] b){
		float[,] res = new float[a.GetLength(0), b.GetLength(1)];
		for (int i=0;i<res.GetLength(0);i++){
			for (int ii=0;ii<res.GetLength(1);ii++){
				float r = 0;
				for (int j = 0;j<a.GetLength(0);j++){
					r += a[i,j]*b[j,ii];
				}
				res[i,ii] = r;
			}
		}
		return res;
	}
	
	// algorithm from "Essential mathematics for Games and Interactive apps"
	public static bool Invert(float[,] A){
		int n = 4;
		// which row have we swapped with the current one?
    	int[] swap  = new int[n];

    	// do one pass for each diagonal element
    	for ( int pivot = 0; pivot < n; ++pivot )
    	{
        	int row, col;  // counters

	        // find the largest magnitude element in the current column
    	    int maxrow = pivot;
        	float maxelem = Mathf.Abs( A[ maxrow, pivot ] );
        	for ( row = pivot+1; row < n; ++row )
        	{
            	float elem = Mathf.Abs( A[ row,pivot ] );
            	if ( elem > maxelem )
            	{
                	maxelem = elem;
                	maxrow = row;
            	}
        	}

        	// if max is zero, stop!
        	if ( Mathf.Abs( maxelem )<Mathf.Epsilon )
        	{
            	// ERROR_OUT( "::Inverse() -- singular matrix\n" );
            	return false;
        	}

        	// if not in the current row, swap rows
        	swap[pivot] = maxrow;
        	if ( maxrow != pivot )
        	{
            	// swap the row
            	for ( col = 0; col < n; ++col )
            	{
                	float temp = A[ maxrow,col ];
                	A[ maxrow ,col ] = A[ pivot,col ];
                	A[ pivot ,col ] = temp;
            	}
        	}
       
        	// multiply current row by 1/pivot to "set" pivot to 1
        	float pivotRecip = 1.0f/A[ pivot, pivot ];
        	for ( col = 0; col < n; ++col )
        	{
            	A[ pivot,col ] *= pivotRecip;
        	}

        	// copy 1/pivot to pivot point (doing inverse in place)
        	A[pivot , pivot] = pivotRecip;

        	// now zero out pivot column in other rows 
        	for ( row = 0; row < n; ++row )
        	{
            	// don't subtract from pivot row
            	if ( row == pivot )
                	continue;
               
            	// subtract multiple of pivot row from current row,
            	// such that pivot column element becomes 0
            	float factor = A[ row,pivot ];

            	// clear pivot column element (doing inverse in place)
            	// will end up setting this element to -factor*pivotInverse
            	A[ row,pivot ] = 0.0f;
            
            	// subtract multiple of row
            	for ( col = 0; col < n; ++col )
            	{
                	A[ row,col ] -= factor*A[ pivot,col ];   
            	}
        	}
    	}

    	// done, undo swaps in column direction, in reverse order
    	int p = n;
    	do
    	{
        	--p;
        	// if row has been swapped
        	if (swap[p] != p)
        	{
           		// swap the corresponding column
            	for (int row = 0; row < n; ++row )
            	{
                	float temp = A[ row,swap[p] ];
                	A[ row ,swap[p] ] = A[ row,p ];
                	A[ row ,p ] = temp;
            	}
        	}
    	}
    	while (p > 0);

    	return true;
	}
	/*
	 * 
// Algorithm From "Essential mathematics for Games and Interactive apps"
bool Matrix::Inverse(Matrix *resultMatrix) {
    *resultMatrix = *this; // copy current matrix to resultMatrix
    unsigned int n = 4;
    float *A = resultMatrix->matrix;
    unsigned int* swap;       // which row have we swapped with the current one?
    swap = new unsigned int[n];

    // do one pass for each diagonal element
    for ( unsigned int pivot = 0; pivot < n; ++pivot )
    {
        unsigned int row, col;  // counters

        // find the largest magnitude element in the current column
        unsigned int maxrow = pivot;
        float maxelem = abs( A[ maxrow + n*pivot ] );
        for ( row = pivot+1; row < n; ++row )
        {
            float elem = abs( A[ row + n*pivot ] );
            if ( elem > maxelem )
            {
                maxelem = elem;
                maxrow = row;
            }
        }

        // if max is zero, stop!
        if ( Mathf::IsZero( maxelem ) )
        {
            // ERROR_OUT( "::Inverse() -- singular matrix\n" );
            delete [] swap;
            return false;
        }

        // if not in the current row, swap rows
        swap[pivot] = maxrow;
        if ( maxrow != pivot )
        {
            // swap the row
            for ( col = 0; col < n; ++col )
            {
                float temp = A[ maxrow + n*col ];
                A[ maxrow + n*col ] = A[ pivot + n*col ];
                A[ pivot + n*col ] = temp;
            }
        }
       
        // multiply current row by 1/pivot to "set" pivot to 1
        float pivotRecip = 1.0f/A[ n*pivot + pivot ];
        for ( col = 0; col < n; ++col )
        {
            A[ pivot + n*col ] *= pivotRecip;
        }

        // copy 1/pivot to pivot point (doing inverse in place)
        A[pivot + n*pivot] = pivotRecip;

        // now zero out pivot column in other rows 
        for ( row = 0; row < n; ++row )
        {
            // don't subtract from pivot row
            if ( row == pivot )
                continue;
               
            // subtract multiple of pivot row from current row,
            // such that pivot column element becomes 0
            float factor = A[ row + n*pivot ];

            // clear pivot column element (doing inverse in place)
            // will end up setting this element to -factor*pivotInverse
            A[ row + n*pivot ] = 0.0f;
            
            // subtract multiple of row
            for ( col = 0; col < n; ++col )
            {
                A[ row + n*col ] -= factor*A[ pivot + n*col ];   
            }
        }
    }

    // done, undo swaps in column direction, in reverse order
    unsigned int p = n;
    do
    {
        --p;
        // if row has been swapped
        if (swap[p] != p)
        {
            // swap the corresponding column
            for ( unsigned int row = 0; row < n; ++row )
            {
                float temp = A[ row + n*swap[p] ];
                A[ row + n*swap[p] ] = A[ row + n*p ];
                A[ row + n*p ] = temp;
            }
        }
    }
    while (p > 0);

    delete [] swap;

    return true;
}
	 * */}
