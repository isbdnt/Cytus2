using UnityEngine;

namespace Cytus2
{
    public class GridViewAnchor
    {
        public int direction { get; private set; }
        private Vector3 _pivot;
        private Vector2Int _origin;
        private int _cellSize;

        public GridViewAnchor(int width, int height, int cellSize, Vector2Int origin, bool upsideDown = false)
        {
            direction = upsideDown ? -1 : 1;
            _pivot = new Vector3(Screen.width, Screen.height) / 2f - new Vector3(width, height * direction) / 2f * cellSize;
            _origin = origin;
            _cellSize = cellSize;
        }

        public Vector2Int ToGridPosition(Vector3 realPos)
        {
            Vector3 pos = (realPos - _pivot) / _cellSize;
            return new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y) * direction) - _origin;
        }

        public Vector3 ToWorldPosition(Vector2Int gridPos)
        {
            return _pivot + new Vector3(gridPos.x + _origin.x, (gridPos.y + _origin.y) * direction) * _cellSize;
        }

        public Vector3 ToWorldPosition(Vector2 gridPos)
        {
            return _pivot + new Vector3(gridPos.x + _origin.x, (gridPos.y + _origin.y) * direction) * _cellSize;
        }
    }
}