﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monad;

namespace DPMiner
{
    public interface IPetriNet
    {
        IPetriControl getControls();
    }
    public interface  IPetriControl
    {
        public bool TryFire(int id);
        public bool Act();
        public int[] Firable();

    }
    struct  PetriNet:IPetriNet
    {
        
        int places;
        int[] markup;
        int tc;
        int[,] transitions;
        
        public PetriNet(int places, int tc)
        {
            if(places<0 || tc < 0)
                throw new ArgumentException("numbers must be positive!");
            this.places = places;
            this.tc = tc;
            markup = new int[places];
            transitions = new int[tc, places];
        }

       public Maybe<PetriNet> setMarkup( int[] markup)
       {
           if(markup == null)
               throw new ArgumentNullException();
           if(markup.Length != places)
               return Maybe<PetriNet>.None();
           this.markup = markup;
           return this;
       }
       public Maybe<PetriNet> setTransitions(int[,] transitions)
       {
           if(transitions == null)
               throw new ArgumentNullException();
           Tuple<int,int> size =Program.Util.MeasureSize(transitions);
           if( size.Item2 != places || size.Item1 != tc )
               return Maybe<PetriNet>.None();
           this.transitions = transitions;
           return this;
       }

      private void Fire( int transID )
      {
          for(int i = 0; i < places; i ++ )
              markup[i]+= transitions[transID, i];
          
      }
      private bool IsFirable(int transID)
      {
          for(int i = 0; i < places; i ++ )
              if(transitions[transID, i] < 0 && markup[i] < -transitions[transID, i])
                  return false;
          return true;
      }
      public IPetriControl GetControls()
      {
          return new PetriControl(this);
      }                                   
      public class PetriControl:IPetriControl
      {
          PetriNet model;
          public PetriControl(PetriNet net)
          {
              model = net;
          }
          public int[] Firable()
          {
              List<int> firable = new List<int>();
             for(int i=0 ; i<model.places; i++ )
                 if(model.IsFirable(i))
                     firable.Add(i);
             return firable.ToArray();
          }
          public bool Tryfire(int id)
          {
              if(model.IsFirable(id))
              {
                  model.Fire(id);
                  return true;
              }
              return false;

         }
        public bool Act()
        {
            Random gen = new Random();
            int[] actions =  Firable();
            if(actions.Length == 0)
                return false;
            model.Fire(gen.Next(0, actions.Length));
            return true;
                
        }

      }
    }
      
    }
}
