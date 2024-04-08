using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Game.Scripts.SteeringBehaviours;
using Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Scripts.UI
{
    public class GameDirectorUI : Menu
    {
        [SerializeField] private StyleSheet style;
        [SerializeField] private bool findAllAgents = true;
        [SerializeField] private List<SteeringAgent> steeringAgents = new();
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        private int _currentAgentIndex = -1;

        private SteeringAgent CurrentAgent => _currentAgentIndex < 0 ? null : steeringAgents[_currentAgentIndex];

        private new void Start()
        {
            base.Start();
            if (virtualCamera == null)
            {
                virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            }

            if (findAllAgents)
            {
                steeringAgents = FindObjectsOfType<SteeringAgent>().ToList();
            }

            if (steeringAgents.Count > 0)
            {
                UpdateVirtualCamera(0);
            }
            
            Debug.Log("Awooga 2: Electric Boogaloo");
        }

        private void UpdateVirtualCamera(int newIndex)
        {
            if (newIndex < 0) newIndex = steeringAgents.Count;

            _currentAgentIndex = newIndex % steeringAgents.Count;
            
            virtualCamera.LookAt = steeringAgents[_currentAgentIndex].transform;
            virtualCamera.Follow = steeringAgents[_currentAgentIndex].transform;

            RefreshUI = true;
        }
        
        protected override IEnumerator Generate()
        {
            yield return null;
            
            var root = Document.rootVisualElement;
            root.Clear();
            root.styleSheets.Add(style);

            var container = root.Create("w-full", "justify-between", "bg-white");

            var topBar = container.Create("w-full", "bg-emerald-900", "p-2", "flex-row", "justify-between", "text-white");
            
            var prevAgentButton = topBar.Create<Button>("bg-emerald-600", "p-4", "w-40", "hover-bg-emerald-800", "transition-all");
            prevAgentButton.text = "< Previous";
            prevAgentButton.RegisterCallback<ClickEvent>(_ => UpdateVirtualCamera(_currentAgentIndex - 1));
            
            var currentAgentDetails = topBar.Create("justify-center");
            var currentAgentName = currentAgentDetails.Create<Label>("text-centered", "text-lg", "text-bold");
            var currentAgentClass = currentAgentDetails.Create<Label>("text-centered");
            var nextAgentButton = topBar.Create<Button>("bg-emerald-600", "p-4", "w-40", "hover-bg-emerald-800", "transition-all");
            nextAgentButton.text = "Next >";
            nextAgentButton.RegisterCallback<ClickEvent>(_ => UpdateVirtualCamera(_currentAgentIndex + 1));

            if (Application.isPlaying)
            {
                currentAgentName.text = CurrentAgent.name;
                currentAgentClass.text = CurrentAgent.transform.parent.name;
            }
            else
            {
                currentAgentName.text = "Agent Name";
                currentAgentClass.text = "Agent Class";
            }

        }
    }
}