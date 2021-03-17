import { Skeleton } from "antd";
import { signIn, signOut, useSession } from "next-auth/client";
import Layout from "../components/Layout";

const OpenIdConnectClient = () => {
  const [session, loading] = useSession();

  if (loading) {
    return (
      <Layout>
        <Skeleton />
      </Layout>
    );
  }

  return (
    <Layout>
      {!session && (
        <>
          Not signed in <br />
          {/* TODO Instead of the provider id `metabase` use a global constant. It must match the one set in `[...nextauth].ts` */}
          <button onClick={() => signIn("metabase")}>Sign in</button>
        </>
      )}
      {session && (
        <>
          Signed in as {JSON.stringify(session, null, 2)}
          <br />
          <button onClick={() => signOut()}>Sign out</button>
        </>
      )}
    </Layout>
  );
};

export default OpenIdConnectClient;
