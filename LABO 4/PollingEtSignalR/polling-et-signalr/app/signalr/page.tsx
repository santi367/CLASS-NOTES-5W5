"use client";

import React, { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import { UselessTask } from "../models/UselessTask";
import TaskView from "../_components/tasks-view";

const HUB_URL = "https://localhost:7289/taskHub";

export default function Home() {

  const [tasks, setTasks] = React.useState<UselessTask[]>([]);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  

  function onTaskToggle(id: number) {
    connectionRef.current?.invoke("CompleteTask", id);
  }

  function handleTaskAdd(taskName: string) {
    if (!taskName.trim()) return;
    connectionRef.current?.invoke("AddTask", taskName);
  }

  useEffect(() => {
    // 1. Create the connection to the Hub
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL)
      .build();

      // 2. Listen for the "TaskList" message
    connection.on("TaskList", (taskList: UselessTask[]) => {
      setTasks(taskList);
    });

    // 3. Connect
    connection.start().catch(err => console.error(err));

    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, []);

  return (
    <div className="p-4">
        <h1>SignalR!</h1>
        <TaskView 
          tasks={tasks} 
          onTaskAdd={handleTaskAdd}
          onTaskToggle={onTaskToggle}
        />
    </div>
  );
}