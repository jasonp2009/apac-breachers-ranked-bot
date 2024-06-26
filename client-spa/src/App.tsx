import React from 'react';
import './App.css';
import { Routes, Route } from "react-router-dom";
import {
  Layout,
  MatchQueue,
  Stats,
  MatchHistory,
  NotFound,
  Account,
  MatchDetailsPage
} from "./pages";
import { RouteConstants } from "./constants";
import {QueryClient, QueryClientProvider} from "react-query";

function App() {
  const queryClient = new QueryClient();
  return (
      <div>
        <QueryClientProvider client={queryClient}>
          <Routes>
            <Route path="/" element={<Layout />}>
              <Route index element={<MatchQueue />} />
              <Route path={RouteConstants.Stats} element={<Stats />} />
              <Route path={RouteConstants.Account} element={<Account />} />
              <Route path={RouteConstants.MatchHistory} element={<MatchHistory />} />
              <Route path={`${RouteConstants.MatchDetails}/:id`} element={<MatchDetailsPage />} />
              <Route path="*" element={<NotFound />} />
            </Route>
          </Routes>
        </QueryClientProvider>
      </div>
  );
}

export default App;
