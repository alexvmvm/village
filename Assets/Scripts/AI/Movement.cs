using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Pathfinding;
using System.Linq;

namespace Village.AI
{

    [RequireComponent(typeof(Seeker))]
    [RequireComponent(typeof(AIPath))]
    public class Movement : MonoBehaviour
    {
        public bool IsStopped
        {
            get { return _aiPath.isStopped; }
            set { _aiPath.isStopped = value; }
        }
        public bool ReachedEndOfPath { get { return _reachedEndOfPath; } }

        public bool FailedToFollowPath { get { return _failedToFollowPath; } }

        public bool IsMoving { get { return _aiPath.hasPath && !_aiPath.reachedEndOfPath; } }
        private AstarPath _aStarPath;
        private AIPath _aiPath;
        private Seeker _seeker;
        private bool _failedToFollowPath;
        private bool _reachedEndOfPath;
        private List<GraphNode> _nodes;
        private float _maxSpeed;
        private List<Vector3> _waypoints;
        private Session _session;
        private Game _game;
        void Awake()
        {
            _session = FindObjectOfType<Session>();
            _game = _session.Game;

            _aiPath = GetComponent<AIPath>();
            _aiPath.orientation = OrientationMode.YAxisForward;
            _aiPath.enableRotation = false;
            _aiPath.gravity = Vector3.zero;
            _aiPath.maxSpeed = 2f;
            _aiPath.pickNextWaypointDist = 0.5f;
            _aiPath.endReachedDistance = 0.8f;

            _aStarPath = FindObjectOfType<AstarPath>();

            _seeker = GetComponent<Seeker>();
            _seeker.startEndModifier.exactStartPoint = StartEndModifier.Exactness.NodeConnection;
            _seeker.startEndModifier.exactEndPoint = StartEndModifier.Exactness.SnapToNode;
            _seeker.traversableTags &= ~(1 << _game.TagFromString("foliage"));
            _seeker.traversableTags &= ~(1 << _game.TagFromString("blocking"));

            _nodes = new List<GraphNode>();
            _maxSpeed = _aiPath.maxSpeed;
            _waypoints = new List<Vector3>();

        }

        void OnDisable()
        {
            _aiPath.isStopped = true;
        }

        void OnEnable()
        {
            _aiPath.isStopped = false;
        }

        // Called before pathfinding is started.
        void PreProcessPath(Path path)
        {
            _failedToFollowPath = false;
            _aiPath.isStopped = false;
        }

        public void CancelCurrentPath()
        {
            // current best way to clear the current path
            _seeker.StartPath(transform.position, transform.position);
        }

        public void SetStopped(bool isStopped)
        {
            _aiPath.isStopped = isStopped;
        }

        public void SetMaxSpeed(float maxSpeed)
        {
            _aiPath.maxSpeed = maxSpeed;
        }

        public void ResetMaxSpeed()
        {
            _aiPath.maxSpeed = _maxSpeed;
        }

        /*
            Find Path
        */

        public bool IsPathPossible(Vector3 position)
        {
            return _session.ZoneGraph.IsPathPossible(transform.position, position);
        }

        public void MoveTo(Vector3 position)
        {
            _reachedEndOfPath = false;
            StartCoroutine(MoveTo(position, (ok) =>
            {
                _reachedEndOfPath = ok;
                if (!ok)
                    _failedToFollowPath = true;
            }));
        }

        IEnumerator MoveTo(Vector3 position, Action<bool> callback)
        {
            // move to resource
            var pathToTarget = _seeker.StartPath(transform.position, position);
            yield return StartCoroutine(FollowPath(pathToTarget, callback));
        }

        // IEnumerator MoveToClosest(IEnumerable<Vector3> positions, Action<bool> callback)
        // {
        //     // move to resource
        //     var pathToTarget = _seeker.StartMultiTargetPath(transform.position, positions.ToArray(), false);
        //     yield return StartCoroutine(FollowPath(pathToTarget, callback));
        // }

        // public IEnumerator FollowCurrentPath(Action<bool> callback)
        // {
        //     yield return StartCoroutine(MoveThroughWaypoints(_waypoints, callback));
        // }

        // IEnumerator MoveThroughWaypoints(IEnumerable<Vector3> waypoints, Action<bool> callback)
        // {
        //     var array = waypoints.ToArray();
        //     for(var i = 0; i < array.Length; i++)
        //     {
        //         var pathCompleted = false;
        //         yield return StartCoroutine(MoveTo(array[i], (ok) => pathCompleted = ok));
        //         if(!pathCompleted)
        //         {
        //             callback(false);
        //             yield break;
        //         }
        //     }
        //     callback(true);
        // }

        IEnumerator FollowPath(Path path, Action<bool> callback)
        {
            // move to resource
            yield return StartCoroutine(path.WaitForPath());

            // failed to get resource
            if (path.error)
            {
                callback(false);
                yield break;
            }

            while (!_aiPath.reachedEndOfPath)
            {
                if (FailedToFollowPath)
                {
                    callback(false);
                    yield break;
                }

                yield return null;
            }

            callback(true);
        }

        void OnDrawGizmosSelected()
        {
            if (_waypoints != null)
            {
                Gizmos.color = Color.green;
                for (var i = 0; i < _waypoints.Count; i++)
                {
                    if (i == 0)
                        continue;
                    Gizmos.DrawLine(_waypoints[i - 1], _waypoints[i]);
                }
            }
        }
    }
}

