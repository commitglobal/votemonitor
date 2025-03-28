import { Button } from "@/components/ui/button";
import { Check, Download, Settings, Smartphone } from "lucide-react";
import { typographyClasses } from "../config/site";

export default function Home() {
  return (
    <>
      <div className="flex min-h-screen flex-col">
        <main className="flex-1">
          <section className="w-full py-12 md:py-24 lg:py-32 xl:py-48">
            <div className="container px-4 md:px-6">
              <div className="flex flex-col items-center justify-center text-center">
                <div className="space-y-4 max-w-[700px]">
                  <h1 className={typographyClasses.h1}>
                    Help democracy by reporting voting irregularities
                  </h1>
                  <p className={typographyClasses.p}>
                    Streamline your workflow, boost productivity, and achieve
                    more together. Start your journey today.
                  </p>
                  <div className="flex flex-col sm:flex-row gap-2 justify-center pt-4">
                    <Button size="lg" className="h-12">
                      Get Started
                    </Button>
                    <Button size="lg" variant="outline" className="h-12">
                      Learn more
                    </Button>
                  </div>
                </div>
              </div>
            </div>
          </section>
          <section className="w-full py-6 md:py-12 lg:py-18">
            <div className="container px-4 md:px-6">
              <div className="flex flex-col items-center justify-center space-y-4 text-center">
                <div className="space-y-2">
                  <h2 className={typographyClasses.h2}>
                    Get involved
                  </h2>
                  <p className={typographyClasses.p}>
                    Follow these simple steps to get started with our
                    application and make the most of its features.
                  </p>
                </div>
              </div>
              <div className="mx-auto py-12">
                <ul className={typographyClasses.ul}>
                  <li className="flex items-start gap-4 p-4 rounded-lg border bg-card">
                    <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-primary text-primary-foreground">
                      <Download className="h-5 w-5" />
                    </div>
                    <div>
                      <h3 className={typographyClasses.h3}>
                        Step 1: Select your country
                      </h3>
                      <p className={typographyClasses.p}>
                        Visit the App Store or Google Play Store and search for
                        our app. Download and install it on your device.
                      </p>
                    </div>
                  </li>
                  <li className="flex items-start gap-4 p-4 rounded-lg border bg-card">
                    <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-primary text-primary-foreground">
                      <Smartphone className="h-5 w-5" />
                    </div>
                    <div>
                      <h3 className={typographyClasses.h3}>
                        Step 2: Select election
                      </h3>
                      <p className={typographyClasses.p}>
                        Open the app and follow the prompts to create a new
                        account or sign in with your existing credentials.
                      </p>
                    </div>
                  </li>
                  <li className="flex items-start gap-4 p-4 rounded-lg border bg-card">
                    <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-primary text-primary-foreground">
                      <Settings className="h-5 w-5" />
                    </div>
                    <div>
                      <h3 className={typographyClasses.h3}>
                        Step 3: Configure Your Settings
                      </h3>
                      <p className={typographyClasses.p}>
                        Customize your profile and preferences in the settings
                        menu to personalize your experience.
                      </p>
                    </div>
                  </li>
                  <li className="flex items-start gap-4 p-4 rounded-lg border bg-card">
                    <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-primary text-primary-foreground">
                      <Check className="h-5 w-5" />
                    </div>
                    <div>
                      <h3 className={typographyClasses.h3}>
                        Step 4: Start Using the App
                      </h3>
                      <p className={typographyClasses.p}>
                        You're all set! Explore the features and functionality
                        of the app to get the most out of your experience.
                      </p>
                    </div>
                  </li>
                </ul>
              </div>
            </div>
          </section>
        </main>
      </div>
    </>
  );
}
