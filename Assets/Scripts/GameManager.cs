using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour {

    public GameState debugState;

    private static GameState _gameState;

    public static GameState GameState {
        get => _gameState;
        set {
            _gameState = value;
            MouseLock.ManageMouse(_gameState);
            OnGameStateChanged?.Invoke(_gameState);
        }
    }

    public static Action<GameState> OnGameStateChanged;

    public static GameManager instance;

    private void Awake() {
        InitializeSingleton();
    }

    private void OnDestroy() {
        //SaveSystem.Save(); //saves what can be saved in this scene
    }

    private void Update() {
        debugState = _gameState;
    }

    private void InitializeSingleton() {
        if (instance == null) {
            instance = this;
            GameState = GameState.Playing;
            //SaveSystem.Load(); //Loads what can be loaded in this scene
        }
    }
}

public enum GameState : byte {
    Playing,
    BuildingTower,
    Paused,
    Dialoguing,
    SeeingAnimation,
    SeeingForgeAnimation,
    InInterface,
    WatchingQuestLog,
    FinishDemo,
    Tutorial,
    Dying,
}
//TODO: Task list shows the number of this line, I'm interested in seeing the total amount of lines my game has, so I will put this in the last line of every script I find.
