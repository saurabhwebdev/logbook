import React, { createContext, useContext, useEffect, useState } from 'react';
import type { ReactNode } from 'react';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useAuth } from './AuthContext';

interface SignalRContextType {
  notificationConnection: HubConnection | null;
  presenceConnection: HubConnection | null;
  isConnected: boolean;
  onlineUsers: string[];
}

const SignalRContext = createContext<SignalRContextType>({
  notificationConnection: null,
  presenceConnection: null,
  isConnected: false,
  onlineUsers: [],
});

export const useSignalR = () => useContext(SignalRContext);

interface SignalRProviderProps {
  children: ReactNode;
}

export const SignalRProvider: React.FC<SignalRProviderProps> = ({ children }) => {
  const { isAuthenticated, user } = useAuth();
  const [notificationConnection, setNotificationConnection] = useState<HubConnection | null>(null);
  const [presenceConnection, setPresenceConnection] = useState<HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  const [onlineUsers, setOnlineUsers] = useState<string[]>([]);

  useEffect(() => {
    if (!isAuthenticated || !user) {
      // Disconnect if not authenticated
      notificationConnection?.stop();
      presenceConnection?.stop();
      setNotificationConnection(null);
      setPresenceConnection(null);
      setIsConnected(false);
      setOnlineUsers([]);
      return;
    }

    const accessToken = localStorage.getItem('accessToken');
    if (!accessToken) return;

    // Build notification hub connection
    const notifHub = new HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_API_BASE_URL}/hubs/notifications`, {
        accessTokenFactory: () => accessToken,
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    // Build presence hub connection
    const presenceHub = new HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_API_BASE_URL}/hubs/presence`, {
        accessTokenFactory: () => accessToken,
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    // Start connections
    Promise.all([notifHub.start(), presenceHub.start()])
      .then(() => {
        console.log('SignalR connections established');
        setNotificationConnection(notifHub);
        setPresenceConnection(presenceHub);
        setIsConnected(true);

        // Get initial online users list
        presenceHub.invoke('GetOnlineUsers').then((users: string[]) => {
          setOnlineUsers(users);
        });
      })
      .catch((err) => {
        console.error('SignalR connection error:', err);
        setIsConnected(false);
      });

    // Setup presence hub listeners
    presenceHub.on('UserOnline', (userId: string) => {
      setOnlineUsers((prev) => [...new Set([...prev, userId])]);
    });

    presenceHub.on('UserOffline', (userId: string) => {
      setOnlineUsers((prev) => prev.filter((id) => id !== userId));
    });

    // Cleanup on unmount
    return () => {
      notifHub.stop();
      presenceHub.stop();
    };
  }, [isAuthenticated, user]);

  return (
    <SignalRContext.Provider
      value={{
        notificationConnection,
        presenceConnection,
        isConnected,
        onlineUsers,
      }}
    >
      {children}
    </SignalRContext.Provider>
  );
};
