namespace Bot
{
	public class TaskManager
	{
		public List<Task> Tasks { get; set; }

		public void StopAll()
		{
			foreach (var task in Tasks)
			{
				task.Dispose();
			}
		}
	}
}
