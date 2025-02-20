import { IncomingMessage, ServerResponse } from "http";
import { useMemo } from "react";
import {
  ApolloClient,
  createHttpLink,
  InMemoryCache,
  NormalizedCacheObject,
} from "@apollo/client";
import merge from "deepmerge";

let apolloClient: ApolloClient<NormalizedCacheObject> | undefined;

export type ResolverContext = {
  req?: IncomingMessage;
  res?: ServerResponse;
};

function createIsomorphLink(context: ResolverContext = {}) {
  return createHttpLink({
    uri:
      // In the case `typeof window === "undefined"`, that is on the server
      // side, because we require HTTPS for the JWT bearer token, we cannot use
      // "http://backend:8080/graphql/" to make local requests. When the reverse
      // proxy NGINX serves the certificate like in development we could use
      // "https://nginx:443/graphql/" to make local requests, which this does
      // however not work in production. Thus, on the server side we also use
      // `${process.env.NEXT_PUBLIC_METABASE_URL}/graphql/` as on the client
      // side.
      typeof window === "undefined"
        ? `${process.env.NEXT_PUBLIC_METABASE_URL}/graphql/`
        : `${process.env.NEXT_PUBLIC_METABASE_URL}/graphql/`,
    useGETForQueries: true,
    credentials: "same-origin",
    headers: {
      cookie: context.req ? context.req.headers.cookie : null,
    },
  });
}

function createApolloClient(context?: ResolverContext) {
  return new ApolloClient({
    ssrMode: typeof window === "undefined",
    link: createIsomorphLink(context),
    cache: new InMemoryCache(),
  });
}

export function initializeApollo(
  initialState: any = null,
  // Pages with Next.js data fetching methods, like `getStaticProps`, can send
  // a custom context which will be used by `SchemaLink` to server render pages
  context?: ResolverContext
) {
  const _apolloClient = apolloClient ?? createApolloClient(context);

  // If your page has Next.js data fetching methods that use Apollo Client, the initial state
  // get hydrated here
  if (initialState) {
    // Get existing cache, loaded during client side data fetching
    const existingCache = _apolloClient.extract();
    // Merge the existing cache into data passed from getStaticProps/getServerSideProps
    const data = merge(initialState, existingCache);
    // Restore the cache with the merged data
    _apolloClient.cache.restore(data);
  }
  // For SSG and SSR always create a new Apollo Client
  if (typeof window === "undefined") return _apolloClient;
  // Create the Apollo Client once in the client
  if (!apolloClient) apolloClient = _apolloClient;

  return _apolloClient;
}

export function useApollo(initialState: any) {
  const store = useMemo(() => initializeApollo(initialState), [initialState]);
  return store;
}
