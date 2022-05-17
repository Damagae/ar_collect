using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rose.Daphne.ARCollect.Utility
{
    /// <summary>
    /// Follow a target (position only). 
    /// </summary>
    /// <remarks>
    /// For rotation see <see cref="Overlap"/>.
    /// </remarks>
    public class FollowPosition : Solver
    {
        /// <inheritdoc />
        public override void SolverUpdate()
        {
            var target = SolverHandler.TransformTarget;
            if (target != null)
            {
                GoalPosition = target.position;
            }
        }
    }
}

