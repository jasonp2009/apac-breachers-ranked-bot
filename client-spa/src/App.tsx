import React from 'react';
import './App.css';
import { Routes, Route } from "react-router-dom";
import { Layout, MatchQueue, Stats, NotFound } from "./pages";

function App() {
  return (
      <div>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index element={<MatchQueue />} />
            <Route path="stats" element={<Stats />} />
            <Route path="*" element={<NotFound />} />
          </Route>
        </Routes>
      </div>
  );
}

export default App;
