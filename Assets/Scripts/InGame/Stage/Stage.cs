
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public struct StageData
{
    public struct WaveData
    {
        public struct SpawnData
        {
            public string Name
            {
                set;
                get;
            }

            public int Min
            {
                set;
                get;
            }

            public int Max
            {
                set;
                get;
            }

            public float Factor
            {
                set;
                get;
            }

            public float Offset
            {
                get;
                set;
            }

            public float Spacing
            {
                get;
                set;
            }
            
            public int GetValue()
            {
                float r = Random.Range(0.0f, 1.0f);
                return Mathf.Min(Min + Mathf.FloorToInt((Max - Min + 1) * (r < (1.0f - Factor) ? r / (1.0f - Factor) * Factor : Factor + (r - (1.0f - Factor)) / Factor * (1.0f - Factor))), Max);
            }

            public SpawnData(string name, int min, int max, float factor, float offset, float spacing)
            {
                Name = name;
                Min = min;
                Max = max;
                Factor = factor;
                Offset = offset;
                Spacing = spacing;
            }

            public SpawnData(JsonData waveData)
            {
                
                Name = (string)waveData["Name"];
                Min = (int)waveData["Min"];
                Max = (int)waveData["Max"];

                if(waveData["Factor"] != null)
                {
                    Factor = (float)(double)waveData["Factor"];
                }
                else
                {
                    Factor = 0.5f;
                }
                
                if(waveData["Offset"] != null)
                {
                    Offset = (float)(double)waveData["Offset"];
                }
                else
                {
                    Offset = 0.0f;
                }

                if(waveData["Spacing"] != null)
                {
                    Spacing = (float)(double)waveData["Spacing"];
                }
                else
                {
                    Spacing = 0.0f;
                }
            }

            public void Spawn()
            {
                int count = GetValue();

                for(int i = 0; i < count; ++i)
                {
                    GameManager.Instance.SpawnEnemy(Name, Offset + Spacing * i);
                }
            }
        }

        SpawnData[] m_WaveData;

        public WaveData(JsonData waveData)
        {
            JsonData spawnData = waveData["SpawnData"];
            m_WaveData = new SpawnData[spawnData.Count];
            for(int i = 0, c = m_WaveData.Length; i < c; ++i)
            {
                m_WaveData[i] = new SpawnData(spawnData[i]);
            }
        }

        public void Spawn()
        {
            for(int i = 0; i < m_WaveData.Length; ++i)
            {
                m_WaveData[i].Spawn();
            }
        }
    }

    WaveData[] m_StageData;
    public StageData(string strJsonData)
    {
        JsonData stageData = JsonMapper.ToObject(strJsonData)["Stage"];

        JsonData waveList = stageData["Wave"];

        m_StageData = new WaveData[waveList.Count];
        for(int i = 0, c = m_StageData.Length; i < c; ++i)
        {
            m_StageData[i] = new WaveData(waveList[i]);
        }
    }

    public void Spawn(int index)
    {
        if(index < m_StageData.Length)
        {
            m_StageData[index].Spawn();
        }
    }
}