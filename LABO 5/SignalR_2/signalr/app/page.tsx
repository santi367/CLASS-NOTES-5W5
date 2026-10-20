"use client"

import { useState, useEffect, useRef, useCallback } from "react"
import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr"

import { Button, BorderedContainer, LoginView } from "ui-exercices-5w5"
import ChatComponent from "@/components/chat/chat"

const serverUrl = "http://localhost:5106/"
const loginUrl = serverUrl + "api/Account"
const hubUrl = serverUrl + "chat"

export default function Home() {

  const [hubConnection, setHubConnection] = useState<HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  
  const handleConnected = useCallback(() => {
    console.log("Connecté au Hub");
    setIsConnected(true);
  }, []);

  function connectToHub() {
    const newHubConnection = new HubConnectionBuilder()
                              .withUrl(hubUrl, { accessTokenFactory: () => sessionStorage.getItem("token")! })
                              .withAutomaticReconnect()
                              .configureLogging(LogLevel.Information)
                              .build();

    // On ne démarre pas la connexion ici : ChatComponent enregistre d'abord
    // ses handlers .on(...) puis démarre la connexion, pour ne manquer aucun
    // message envoyé par le serveur juste après la connexion au Hub.
    setHubConnection(newHubConnection);
  }

  function logout() {
    console.log("L'utilisateur se déconnecte, on arrête le HubConnection");
    if(hubConnection){
      hubConnection.stop();
      setHubConnection(null);
    }
    setIsConnected(false);
  }

  function RenderContent(){
    if(!hubConnection){
      return (
        <div>
          <div >Pas connecté au Hub..</div>
          <br></br>
          <Button variant="secondary" onClick={connectToHub}>Se connecter au Hub</Button>
        </div>
      );
    }
    else{
      return (
        <div>
          <div>{isConnected ? "Connecté!" : "Connexion en cours..."}</div>
          <ChatComponent hubConnection={hubConnection} onConnected={handleConnected} />
        </div>
      );
    }
  }

  return (
    <div>
      <h1 className="text-3xl font-bold mb-2 p-2">Chat SignalR</h1>
      <div className="p-2 max-w-[1400px]">
        <LoginView apiUrl={loginUrl} onLogout={logout} />
        <BorderedContainer className="p-6 mt-2">
          {RenderContent()}
        </BorderedContainer>
      </div>
    </div>
  );
}
