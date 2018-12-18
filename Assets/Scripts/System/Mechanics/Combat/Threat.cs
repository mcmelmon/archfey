using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Threat : MonoBehaviour {

    private List<Threats> threats = new List<Threats>();

    public struct Threats
    {
        public readonly GameObject attacker;
        public float threat;

        public Threats(GameObject attacker, float threat)
        {
            this.attacker = attacker;
            this.threat = threat;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Threats))
            {
                return false;
            }

            var threats = (Threats)obj;
            return EqualityComparer<GameObject>.Default.Equals(attacker, threats.attacker);
        }

        public override int GetHashCode()
        {
            return 1619755646 + EqualityComparer<GameObject>.Default.GetHashCode(attacker);
        }

        public static bool operator ==(Threats t1, Threats t2)
        {
            if (t1.attacker == t2.attacker)
            {
                return t1.Equals(t2);
            }
            return false;
        }

        public static bool operator !=(Threats t1, Threats t2)
        {
            if (t1.attacker != t2.attacker)
            {
                return !t1.Equals(t2);
            }
            return false;
        }
    }


    // Unity

    private void Start()
    {
        StartCoroutine(PruneThreats());
    }


    // public


    public void AddThreat(GameObject _attacker, float _damage)
    {
        Threats _threat = new Threats(_attacker, _damage);

        if (!threats.Contains(_threat)) {
            threats.Add(_threat);
        } else {
            _threat = threats.Find(threats => threats.attacker == _attacker);
            _threat.threat += _damage;
        }

        threats.Sort((x, y) => x.threat.CompareTo(y.threat));
    }


    public GameObject BiggestThreat()
    {
        threats.Sort((x, y) => x.threat.CompareTo(y.threat));
        return threats[0].attacker;
    }


    public List<Threats> GetThreats()
    {
        threats.Sort((x, y) => x.threat.CompareTo(y.threat));
        return threats;
    }


    public bool IsAThreat(GameObject _unit)
    {
        return threats.Find(threats => threats.attacker == _unit).attacker != null;
    }


    // private


    private IEnumerator PruneThreats()
    {
        while (true) {
            yield return new WaitForSeconds(Turn.action_threshold);
            threats.FindAll(threats => threats.attacker == null).Clear();
        }
    }
}
