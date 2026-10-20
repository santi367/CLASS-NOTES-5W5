"use client";

import React, { useEffect } from "react";
import axios from "axios";
import TaskView from "../_components/tasks-view";
import { UselessTask } from "../models/UselessTask";
const API_URL = "https://localhost:7289/api/UselessTasks";


export default function Home() {

  const [tasks, setTasks] = React.useState<UselessTask[]>([]);

  async function handleTaskAdd(taskName: string) {
    await axios.post(`${API_URL}/Add`, null, { params: { taskText: taskName } });
    updateTasks();
  }

  async function onTaskToggle(id: number) {
    await axios.get(`${API_URL}/Complete/${id}`);
    updateTasks();
  }

  async function updateTasks() {
    const response = await axios.get<UselessTask[]>(`${API_URL}/GetAll`);
    setTasks(response.data);
  }

  useEffect(() => {
    updateTasks();
    const interval = setInterval(updateTasks, 1000);
    return () => clearInterval(interval);
  }, []);
  
  return (
    <div className="p-4">
        <h1>Polling!</h1>
        <TaskView 
          tasks={tasks} 
          onTaskAdd={handleTaskAdd}
          onTaskToggle={onTaskToggle}
        />
    </div>

  );
}