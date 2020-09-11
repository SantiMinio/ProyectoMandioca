﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using U = Utility;

namespace GOAP
{
    public class AStarNormal<Node> where Node : class
    {
        //Podría guardar ambas cosas en una tupla, pero al crear una clase custom me da mas legibilidad abajo
        public class Arc
        {
            public Node endpoint;
            public float cost;
            public Arc(Node ep, float c)
            {
                endpoint = ep;
                cost = c;
            }
        }

        //TimeSlicing 1 - Aca cambie el AStar a que sea un IEnumerator y tenga un callback al finalizar
        //en este caso, se hace una pausa cada 10 Nodos Chequeados, pero podiras tenes un if con  un tiempo y hacerlo solo en ese momento por ejemplo

        //expand can return null as "no neighbours"
        public static IEnumerator Run
        (
            Node from,
            //Node to,      //Lo saque ya que la heuristica ahora no utiliza el to de goal final
            Func<Node, float> h,                //Current, Goal -> Heuristic cost
            Func<Node, bool> satisfies,             //Current -> Satisfies
            Func<Node, IEnumerable<Arc>> expand,    //Current -> (Endpoint, Cost)[]
            Action<IEnumerable<Node>> callback
        )
        {
            int watchdog = 0;

            var initialState = new AStarState<Node>();
            initialState.open.Add(from);
            initialState.gs[from] = 0;
            initialState.fs[from] = h(from);
            initialState.previous[from] = null;
            initialState.current = from;

            var state = initialState;
            while (state.open.Count > 0 && !state.finished)
            {
                //Debugger gets buggy af with this, can't watch variable:
                state = state.Clone();

                var candidate = state.open.OrderBy(x => state.fs[x]).First();
                state.current = candidate;

                //DebugGoap(state);

                if (satisfies(candidate))
                {
                    U.Log("SATISFIED");
                    state.finished = true;
                }
                else
                {
                    state.open.Remove(candidate);
                    state.closed.Add(candidate);
                    var neighbours = expand(candidate);
                    if (neighbours == null || !neighbours.Any())
                        continue;

                    var gCandidate = state.gs[candidate];

                    foreach (var ne in neighbours)
                    {
                        if (ne.endpoint.In(state.closed))
                            continue;

                        var gNeighbour = gCandidate + ne.cost;
                        state.open.Add(ne.endpoint);

                        if (gNeighbour > state.gs.DefaultGet(ne.endpoint, () => gNeighbour))
                            continue;

                        state.previous[ne.endpoint] = candidate;
                        state.gs[ne.endpoint] = gNeighbour;
                        state.fs[ne.endpoint] = gNeighbour + h(ne.endpoint);
                    }
                }
                watchdog++;//TimeSlicing 1 - Sumo 1 a los nodos que mire

                if (watchdog % 10 == 0)//TimeSlicing 1 - Si los que mire es multiplo de 10 entonces hago una pausa
                {
                    yield return null;//TimeSlicing 1 - Aca se hace una pausa y se retoma al frame siguiente, dejando que el juego siga corriendo
                }
            }

            if (!state.finished)
            {
                callback(null);//TimeSlicing 1 - se llama al callback para mandar el path, en este caso no encontro
            }
            else
            {
                //Climb reversed tree.
                var seq =
                    U.Generate(state.current, n => state.previous[n])
                    .TakeWhile(n => n != null)
                    .Reverse();

                callback(seq);//TimeSlicing 1 - se llama al callback para mandar el path
            }

        }

        static void DebugGoap(AStarState<Node> state)
        {
            var candidate = state.current;
            U.Log("OPEN SET " + state.open.Aggregate("", (a, x) => a + x.ToString() + "\n\n"));
            U.Log("CLOSED SET " + state.closed.Aggregate("", (a, x) => a + x.ToString() + "\n\n"));
            U.Log("CHOSEN CANDIDATE COST " + state.fs[candidate] + ":" + candidate.ToString());
            if (state is AStarState<GoapState>)
            {
                U.Log("SEQUENCE FOR CANDIDATE" +
                    U.Generate(state.current, n => state.previous[n])
                        .TakeWhile(x => x != null)
                        .Reverse()
                        .Select(x => x as GoapState)
                        .Where(x => x != null && x.generatingAction != null)
                        .Aggregate("", (a, x) => a + "-->" + x.generatingAction.Name)
                );

                var prevs = state.previous as Dictionary<GoapState, GoapState>;
                U.Log("Other candidate chains:\n"
                    + prevs
                        .Select(kv => kv.Key)
                        .Where(y => !prevs.ContainsValue(y))
                        .Aggregate("", (a, y) => a +
                            U.Generate(y, n => prevs[n])
                                .TakeWhile(x => x != null)
                                .Reverse()
                                .Select(x => x as GoapState)
                                .Where(x => x != null && x.generatingAction != null)
                                .Aggregate("", (a2, x) => a2 + "-->" + x.generatingAction.Name + "(" + x.step + ")")
                            + " (COST: g" + (state.gs)[y as Node] + "   f" + state.fs[y as Node] + ")"
                            + "\n"
                        )
                );
            }
        }
    }
}
