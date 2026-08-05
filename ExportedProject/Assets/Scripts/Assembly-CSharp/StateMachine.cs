using System;
using System.Collections.Generic;

public sealed class StateMachine<TLabel>
{
	private class State
	{
		public readonly Action onEnter;

		public readonly Action onUpdate;

		public readonly Action onExit;

		public readonly TLabel label;

		public State(TLabel label, Action onEnter, Action onUpdate, Action onExit)
		{
			this.onEnter = onEnter;
			this.onUpdate = onUpdate;
			this.onExit = onExit;
			this.label = label;
		}
	}

	private readonly Dictionary<TLabel, State> stateDictionary;

	private State m_currentState;

	private State m_previousState;

	public TLabel CurrentState
	{
		get
		{
			return m_currentState.label;
		}
		set
		{
			ChangeState(value);
		}
	}

	public TLabel PreviousState
	{
		get
		{
			return m_previousState.label;
		}
	}

	public StateMachine()
	{
		stateDictionary = new Dictionary<TLabel, State>();
	}

	public void AddState(TLabel label, Action onEnter, Action onUpdate, Action onExit)
	{
		stateDictionary[label] = new State(label, onEnter, onUpdate, onExit);
	}

	private void ChangeState(TLabel newState)
	{
		if (m_currentState != null && m_currentState.onExit != null)
		{
			m_currentState.onExit();
		}
		m_previousState = m_currentState;
		m_currentState = stateDictionary[newState];
		if (m_currentState.onEnter != null)
		{
			m_currentState.onEnter();
		}
	}

	public void Update()
	{
		if (m_currentState != null && m_currentState.onUpdate != null)
		{
			m_currentState.onUpdate();
		}
	}
}
