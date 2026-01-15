using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class Quest
{
    public int id;
    public string name;
    public string description;
    public int levelRequired;
    public int rewardExp;
    public int rewardGold;
    public string status;
    public List<QuestObjective> objectives;
}

[System.Serializable]
public class QuestObjective
{
    public string type;
    public string target;
    public int required;
    public int current;
    public bool completed;
}

public class QuestSystem : MonoBehaviour
{
    private List<Quest> _activeQuests = new List<Quest>();
    private List<Quest> _completedQuests = new List<Quest>();
    
    public UnityEvent<Quest> OnQuestAccepted = new UnityEvent<Quest>();
    public UnityEvent<Quest> OnQuestCompleted = new UnityEvent<Quest>();
    public UnityEvent<QuestObjective> OnObjectiveUpdated = new UnityEvent<QuestObjective>();
    
    private static QuestSystem _instance;
    
    public static QuestSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("QuestSystem");
                _instance = obj.AddComponent<QuestSystem>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void AcceptQuest(Quest quest)
    {
        if (_activeQuests.Find(q => q.id == quest.id) != null)
        {
            Debug.LogWarning("Quest já está ativa");
            return;
        }
        
        quest.status = "in_progress";
        _activeQuests.Add(quest);
        OnQuestAccepted.Invoke(quest);
        Debug.Log($"Quest aceita: {quest.name}");
    }
    
    public void UpdateObjective(int questId, string objectiveType, int amount)
    {
        Quest quest = _activeQuests.Find(q => q.id == questId);
        if (quest == null)
            return;
        
        QuestObjective objective = quest.objectives.Find(o => o.type == objectiveType);
        if (objective == null)
            return;
        
        objective.current += amount;
        if (objective.current >= objective.required)
        {
            objective.current = objective.required;
            objective.completed = true;
        }
        
        OnObjectiveUpdated.Invoke(objective);
        
        if (IsQuestComplete(quest))
        {
            CompleteQuest(quest);
        }
    }
    
    private bool IsQuestComplete(Quest quest)
    {
        foreach (var objective in quest.objectives)
        {
            if (!objective.completed)
                return false;
        }
        return true;
    }
    
    public void CompleteQuest(Quest quest)
    {
        quest.status = "completed";
        _activeQuests.Remove(quest);
        _completedQuests.Add(quest);
        OnQuestCompleted.Invoke(quest);
        Debug.Log($"Quest completada: {quest.name}");
    }
    
    public List<Quest> GetActiveQuests() => _activeQuests;
    public List<Quest> GetCompletedQuests() => _completedQuests;
    public Quest GetQuestById(int id) => _activeQuests.Find(q => q.id == id);
}
