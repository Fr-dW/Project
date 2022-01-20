using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParticuleSnow{
	public class Particule
	{
		public Vector3 pos,vit;
		public float mas,vol,dens,weight;
		public Matrix<double> fEl, fPl;



		public Particule(Vector3 position,float masse, Vector3 vitesse, Matrix<double> mE, Matrix<double> mP)
		{
			// Init selon les valeurs des sphere
			this.pos = position;
			this.mas= masse;
			this.vit = vitesse;

			// init de base
			this.fEl =mE;

			this.fPl = mP;

			this.dens = 0;
			this.vol = 0;
			this.weight=0;
		}

		public void setWeight(float w)
		{
			this.weight = w;
		}	

		public void resetWeight()
		{
			this.weight = 0;
		}


		public void InitVolDens(float masseGrid,float cellSize)
		{
			this.dens = (masseGrid * weight) / (cellSize * cellSize * cellSize);
	        this.vol = this.mas / this.dens;
		}


		//public void updateVit(float alpha,Vector3 v,Vector3 oldV)
		public void updateVit(float alpha,Vector3 PIC,Vector3 FLIP)
		{
			this.vit = (1-alpha) * PIC + alpha * FLIP;
		}

		public void updatePos(float dt)
		{
			this.pos = this.pos+ dt*this.vit;
		}

		public void updateFela(Matrix<double> f)
		{
			this.fEl = f;
		}

		public void updateFpl(Matrix<double> f)
		{
			this.fPl = f;
		}

	}
}