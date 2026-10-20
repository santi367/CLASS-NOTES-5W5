"use client";

import { useState } from "react";
import axios from "axios";
import api from "@/lib/api";

const USERNAME = "Test5";
const PASSWORD = "Test5Test5Test5!";
const EMAIL = "test5@test5.com";

function messageErreur(error: unknown) {
  if (axios.isAxiosError(error)) {
    return `Erreur ${error.response?.status ?? error.message}`;
  }
  return `Erreur: ${error}`;
}

export default function Home() {
  const [resultat, setResultat] = useState("");
  const [nouvelleDonnee, setNouvelleDonnee] = useState("");

  async function testPublic() {
    try {
      const response = await api.get("/Account/PublicTest");
      setResultat(JSON.stringify(response.data));
    } catch (error) {
      setResultat(messageErreur(error));
    }
  }

  async function testPrive() {
    try {
      const response = await api.get("/Account/PrivateTest");
      setResultat(JSON.stringify(response.data));
    } catch (error) {
      setResultat(messageErreur(error));
    }
  }

  async function register() {
    try {
      const response = await api.post("/Account/Register", {
        username: USERNAME,
        email: EMAIL,
        password: PASSWORD,
        passwordConfirm: PASSWORD,
      });
      sessionStorage.setItem("token", response.data.token);
      setResultat("Enregistré et connecté.");
    } catch (error) {
      setResultat(messageErreur(error));
    }
  }

  async function login() {
    try {
      const response = await api.post("/Account/Login", {
        username: USERNAME,
        password: PASSWORD,
      });
      sessionStorage.setItem("token", response.data.token);
      setResultat("Connecté.");
    } catch (error) {
      setResultat(messageErreur(error));
    }
  }

  function logout() {
    sessionStorage.removeItem("token");
    setResultat("Déconnecté.");
  }

  async function ajouterData() {
    try {
      await api.post("/TestData/CreateData", { name: nouvelleDonnee });
      setResultat(`Ajouté: ${nouvelleDonnee}`);
      setNouvelleDonnee("");
    } catch (error) {
      setResultat(messageErreur(error));
    }
  }

  return (
    <main className="m-4 border-2 border-black bg-white p-2 text-black">
      <div className="flex gap-1">
        <button onClick={testPublic} className="rounded-lg bg-zinc-900 px-3 py-1.5 text-sm font-semibold text-white hover:bg-zinc-700">
          Test Publique
        </button>
        <button onClick={testPrive} className="rounded-lg bg-red-100 px-3 py-1.5 text-sm text-red-600 hover:bg-red-200">
          Test Privé
        </button>
        <button onClick={register} className="rounded-lg bg-blue-600 px-3 py-1.5 text-sm font-semibold text-white hover:bg-blue-500">
          S&apos;enregistrer
        </button>
        <button onClick={login} className="rounded-lg bg-green-600 px-3 py-1.5 text-sm font-semibold text-white hover:bg-green-500">
          Se connecter
        </button>
        <button onClick={logout} className="rounded-lg bg-gray-400 px-3 py-1.5 text-sm font-semibold text-white hover:bg-gray-300">
          Logout
        </button>
      </div>
      <p className="mt-2">Résultat: {resultat}</p>

      <div className="mt-4 border-t pt-2">
        <p className="mb-1 font-semibold">Ajout de data</p>
        <input
          value={nouvelleDonnee}
          onChange={(e) => setNouvelleDonnee(e.target.value)}
          placeholder="N'importe quel texte"
          className="mb-2 w-full border px-2 py-1"
        />
        <button onClick={ajouterData} className="rounded-lg bg-black px-3 py-1.5 text-sm font-semibold text-white hover:bg-zinc-700">
          Ajouter
        </button>
      </div>
    </main>
  );
}