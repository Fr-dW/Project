using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;


public class createParticule: MonoBehaviour
{
    public Rigidbody prefab;
    public Transform spawner;
    public int nbSphere;
    public float rayonSnowBall;

    public Rigidbody[] lParticule;
    public Matrix<double>[] fEl, fPl;

    void Start()
    {
        lParticule = new Rigidbody[nbSphere];
        fEl = new Matrix<double>[nbSphere];
        fPl = new Matrix<double>[nbSphere];
        for (int i = 0; i < nbSphere; i++)
        {
            Vector3 newPos = spawner.position;
            if (i != 0)
            {
                newPos+= Random.insideUnitSphere * rayonSnowBall;
            }
                
            lParticule[i] = Instantiate(prefab, newPos, Quaternion.identity) as Rigidbody;
            fEl[i] = DenseMatrix.OfArray(new[,] {
                {1.0,0.0,0.0},
                {0.0,1.0,0.0},
                {0.0,0.0,1.0}
                });
            fPl[i] = DenseMatrix.OfArray(new[,] {
                {1.0,0.0,0.0},
                {0.0,1.0,0.0},
                {0.0,0.0,1.0}
                });
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public Rigidbody[] getList()
    {
        return lParticule;
    }

    public Matrix<double>[] getFel()
    {
        return fEl;
    }

    public Matrix<double>[] getFpl()
    {
        return fPl;
    }

    public void setList(Rigidbody[] r)
    {
        lParticule = r;
    }

    public void setFel(Matrix<double>[] m )
    {
        fEl = m;
    }

    public void setFpl( Matrix<double>[] m )
    {
        fPl = m;
    }



}
