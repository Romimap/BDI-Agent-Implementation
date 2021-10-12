using System;
using System.Collections.Generic;

public class PathFinding {
    class Node {
        public Coord Coord;
        public float FScore;
        public float GScore;
        public Node CameFrom;

        public Node (int x, int y, Coord start) {
            Coord = new Coord(x, y);
            GScore = float.PositiveInfinity;
            FScore = Coord.distance(start);
            CameFrom = null;
        }

        public int CompareTo(Node compareNode) {
            if (compareNode == null)
                return 1;
            return this.FScore.CompareTo(compareNode.FScore);
        }
    }

    private static List<Coord> ReconstructPath (Node current) {
        List<Coord> ans = new List<Coord>();
        while (current != null) {
            ans.Add(current.Coord);
            current = current.CameFrom;
        }
        ans.Reverse();
        return ans;
    }

    public static List<Coord> PathFind (WorldState worldState, Coord start, Coord end) {
        List<Node> openSet = new List<Node>();

        int w = worldState.Width;
        int h = worldState.Height;
        
        Node[,] map = new Node[w, h];
        for (int x = 0; x < w; x++) {
            for (int y = 0; y < h; y++) {
                map[x, y] = new Node(x, y, start);
            }
        }
        
        map[start.X, start.Y].GScore = 0;
        map[start.X, start.Y].FScore = 0;

        openSet.Add(map[start.X, start.Y]);

        while (openSet.Count > 0) {
            openSet.Sort();
            Node current = openSet[0];

            if (current.Coord.Equals(end)) {
                return ReconstructPath(current.CameFrom);
            }

            openSet.Remove(current);

            List<Node> neighbours = new List<Node>();
            if (current.Coord.X > 0) neighbours.Add(map[current.Coord.X - 1, current.Coord.Y]);
            if (current.Coord.X < w - 1) neighbours.Add(map[current.Coord.X + 1, current.Coord.Y]);
            if (current.Coord.Y > 0) neighbours.Add(map[current.Coord.X, current.Coord.Y - 1]);
            if (current.Coord.Y < h - 1) neighbours.Add(map[current.Coord.X, current.Coord.Y + 1]);

            foreach (Node neighbour in neighbours) {
                float tentativeGscore = current.GScore + 1;
                if (tentativeGscore < neighbour.GScore) {
                    neighbour.CameFrom = current;
                    neighbour.GScore = tentativeGscore;
                    neighbour.FScore = neighbour.GScore + neighbour.Coord.distance(start);
                    if (!openSet.Contains(neighbour)) {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return null;
    }


}