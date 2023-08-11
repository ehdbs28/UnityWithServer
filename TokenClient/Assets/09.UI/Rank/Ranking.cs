using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Ranking : WindowUI
{
    private ScrollView _rankScroll;
    private VisualTreeAsset _rankTemplete;
    
    public Ranking(VisualElement root, VisualTreeAsset rankTemplete) : base(root)
    {
        _rankTemplete = rankTemplete;
        _rankScroll = root.Q<ScrollView>("ranks");
        
        root.Q<Button>("closeBtn").RegisterCallback<ClickEvent>(e => Close());
    }

    private void LoadDB()
    {
        if (NetworkManager.Instance == null)
            return;
        
        NetworkManager.Instance.GetRequest("rank", "", (type, msg) =>
        {
            if (type == MessageType.SUCCESS)
            {
                _rankScroll.Clear();
                RankList list = JsonUtility.FromJson<RankList>(msg);
                for (int i = 1; i <= list.list.Count; i++)
                {
                    RankVO vo = list.list[i - 1];
                    VisualElement itemRoot = _rankTemplete.Instantiate().Q("rankItem");
                    RankItem item = new RankItem(itemRoot, i.ToString(), vo.user_name, vo.score.ToString("D3"), vo.memo);
                    _rankScroll.Add(itemRoot);
                }
            }
        });
    }

    public override void Open()
    {
        LoadDB();
        base.Open();
    }
}
