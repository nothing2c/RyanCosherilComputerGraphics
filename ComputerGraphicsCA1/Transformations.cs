using UnityEngine;
using System.Collections;
using System;

public class Transformations : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Vector3[] cube = new Vector3[8];
        cube[0] = new Vector3(1, 1, 1);
        cube[1] = new Vector3(-1, 1, 1);
        cube[2] = new Vector3(-1, -1, 1);
        cube[3] = new Vector3(1, -1, 1);
        cube[4] = new Vector3(1, 1, -1);
        cube[5] = new Vector3(-1, 1, -1);
        cube[6] = new Vector3(-1, -1, -1);
        cube[7] = new Vector3(1, -1, -1);

        Vector3 startingAxis = new Vector3(14, 3, 3);
        startingAxis.Normalize();
        Quaternion rotation = Quaternion.AngleAxis(-22, startingAxis);
        Matrix4x4 rotationMatrix =
            Matrix4x4.TRS(new Vector3(0,0,0),
                            rotation,
                            Vector3.one);
        //printMatrix(rotationMatrix);

        Vector3[] imageAfterRotation =
            MatrixTransform(cube, rotationMatrix);
        //printVerts(imageAfterRotation);

        //Scale

        Matrix4x4 scaleMatrix =
            Matrix4x4.TRS(new Vector3(0, 0, 0),
                            Quaternion.identity,
                            new Vector3(14,4,3));
        //printMatrix(scaleMatrix);

        Vector3[] imageAfterScale =
            MatrixTransform(imageAfterRotation, scaleMatrix);
        //printVerts(imageAfterScale);

        //Translation

        Matrix4x4 translateMatrix =
            Matrix4x4.TRS(new Vector3(3, -3, 4),
                            Quaternion.identity,
                            Vector3.one);
        //printMatrix(translateMatrix);

        Vector3[] imageAfterTranslate =
            MatrixTransform(imageAfterScale, translateMatrix);
        //printVerts(imageAfterTranslate);

        // matrix x

        Matrix4x4 transformMatrix = translateMatrix * scaleMatrix * rotationMatrix;
        //printMatrix(transformMatrix);

        Vector3[] imageAfterTransform =
            MatrixTransform(cube, transformMatrix);
        //printVerts(imageAfterTransform);

        //Viewing

        Vector3 cameraPosition = new Vector3(16, 6, 53);
        Vector3 cameraLookAt = new Vector3(3, 14, 3);
        Vector3 cameraUp = new Vector3(4, 3, 14);

        Vector3 lookRotationDir = cameraLookAt - cameraPosition;

        Quaternion camRotation = Quaternion.LookRotation(lookRotationDir.normalized, cameraUp.normalized);

        Matrix4x4 viewingMatrix = Matrix4x4.TRS(-cameraPosition, camRotation, Vector3.one);
        //printMatrix(viewingMatrix);

        Vector3[] viewingImage = MatrixTransform(imageAfterTransform, viewingMatrix);
        //printVerts(viewingImage);

        //Projection

        Matrix4x4 projectionMatrix = Matrix4x4.Perspective(45, 1.6f, 1, 1000);
        //printMatrix(projectionMatrix);

        Vector3[] projectionImage = MatrixTransform(viewingImage, projectionMatrix);
        //printVerts(projectionImage);

        Matrix4x4 BIGMATRIX = projectionMatrix * viewingMatrix * transformMatrix;
        printMatrix(BIGMATRIX);

        Vector3[] finalImage = MatrixTransform(cube, BIGMATRIX);
        printVerts(finalImage);
    }

    private void printVerts(Vector3[] newImage)
    {
        for (int i = 0; i < newImage.Length; i++)
            print(newImage[i].x + " , " +
                newImage[i].y + " , " +
                newImage[i].z);

        Debug.Log("printVerts");

    }

    private Vector3[] MatrixTransform(
        Vector3[] meshVertices, 
        Matrix4x4 transformMatrix)
    {
        Vector3[] output = new Vector3[meshVertices.Length];
        for (int i = 0; i < meshVertices.Length; i++)
            output[i] = transformMatrix * 
                new Vector4( 
                meshVertices[i].x,
                meshVertices[i].y,
                meshVertices[i].z,
                    1);

        return output;
    }

    private void printMatrix(Matrix4x4 matrix)
    {
        for (int i = 0; i < 4; i++)
            print(matrix.GetRow(i).ToString());

        Debug.Log("print Matrix");
    }



    // Update is called once per frame
    void Update () {
	
	}
}
