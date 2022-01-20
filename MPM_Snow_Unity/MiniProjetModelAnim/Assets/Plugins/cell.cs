using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellSnow{
	public class Cell{

		public float mas,derivWeight;
		public Vector3 vit,oldVit,f;
		//public bool hasParticle;

		public Cell()
		{
			this.mas = 0;
			this.vit = new Vector3(0,0,0);
			this.oldVit = new Vector3(0,0,0);
			this.f = new Vector3(0,0,0);
			//this.hasParticle =false;
		}


		public void Init(Vector3 vitPart, float masse, float w)
		{
			this.mas = masse * w;
	        this.vit = (vitPart* masse * w) / this.mas;

	        //this.hasParticle = true;
		}

		public void setWeight(float w)
		{
			this.derivWeight = w;
		}


		public void updateForce(Matrix<double> fEla, Matrix<double> fPl,float vol, float mu, float lambda,float c_hard)
		{
			float detE = (float) fEla.Determinant() ;
			float detP = (float) fPl.Determinant() ;

			float J = detE * detP;

			Matrix<double> FelaT = fEla.Transpose();
			Matrix<double> identity = Matrix<double>.Build.DenseIdentity(3);

			var svd = fEla.Svd();
			var U = svd.U;
			var V = svd.VT;
			var S = svd.S;

			var RE = U*V.Transpose();
			var SE = V*S*V.Transpose();

			float newMu =(float) (mu * System.Math.Exp(c_hard*(1-detP)));
			float newlambda = (float) (lambda * System.Math.Exp(c_hard*(1-detP)));


			Matrix<double> sigma = (2.0*newMu) / J * (detE-1) * detE * identity;
			Vector<double> temp = Vector<double>.Build.Dense(new double[] { 1.0, 1.0, 1.0 }); 
			temp =  temp * ( -vol * sigma * this.derivWeight);


			this.f.x = (float) temp[0];
			this.f.y = (float) temp[1];
			this.f.z = (float) temp[2];
		}

		public void updateVit(float dt)
		{
			this.oldVit = this.vit;
			this.vit = this.vit + (dt*this.f)/this.mas;
		}

	}
}
