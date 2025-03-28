import { typographyClasses } from "../config/site";

export default function About() {
  return (
    <div>
      <h1 className={typographyClasses.h1}>
        The Joke Tax Chronicles
      </h1>
      <p className={typographyClasses.p}>
        Once upon a time, in a far-off land, there was a very lazy king who
        spent all day lounging on his throne. One day, his advisors came to him
        with a problem: the kingdom was running out of money.
      </p>
      <h2 className={typographyClasses.h2}>
        The King's Plan
      </h2>
      <p className={typographyClasses.p}>
        The king thought long and hard, and finally came up with
        <a
          href="#"
          className={typographyClasses.a}
        >
          a brilliant plan
        </a>
        : he would tax the jokes in the kingdom.
      </p>
      <blockquote className={typographyClasses.blockquote}>
        "After all," he said, "everyone enjoys a good joke, so it's only fair
        that they should pay for the privilege."
      </blockquote>
      <h3 className={typographyClasses.h3}>
        The Joke Tax
      </h3>
      <p className={typographyClasses.p}>
        The king's subjects were not amused. They grumbled and complained, but
        the king was firm:
      </p>
      <ul className={typographyClasses.ul}>
        <li>1st level of puns: 5 gold coins</li>
        <li>2nd level of jokes: 10 gold coins</li>
        <li>3rd level of one-liners : 20 gold coins</li>
      </ul>
      <p className={typographyClasses.p}>
        As a result, people stopped telling jokes, and the kingdom fell into a
        gloom. But there was one person who refused to let the king's
        foolishness get him down: a court jester named Jokester.
      </p>
      <h3 className={typographyClasses.h3}>
        Jokester's Revolt
      </h3>
      <p className={typographyClasses.p}>
        Jokester began sneaking into the castle in the middle of the night and
        leaving jokes all over the place: under the king's pillow, in his soup,
        even in the royal toilet. The king was furious, but he couldn't seem to
        stop Jokester.
      </p>
      <p className={typographyClasses.p}>
        And then, one day, the people of the kingdom discovered that the jokes
        left by Jokester were so funny that they couldn't help but laugh. And
        once they started laughing, they couldn't stop.
      </p>
      <h3 className={typographyClasses.h3}>
        The People's Rebellion
      </h3>
      <p className={typographyClasses.p}>
        The people of the kingdom, feeling uplifted by the laughter, started to
        tell jokes and puns again, and soon the entire kingdom was in on the
        joke.
      </p>
      <div className="my-6 w-full overflow-y-auto">
        <table className={typographyClasses.table}>
          <thead>
            <tr className={typographyClasses.tr}>
              <th className={typographyClasses.th}>
                King's Treasury
              </th>
              <th className={typographyClasses.th}>
                People's happiness
              </th>
            </tr>
          </thead>
          <tbody>
            <tr className={typographyClasses.tr}>
              <td className={typographyClasses.td}>
                Empty
              </td>
              <td className={typographyClasses.td}>
                Overflowing
              </td>
            </tr>
            <tr className={typographyClasses.tr}>
              <td className={typographyClasses.td}>
                Modest
              </td>
              <td className={typographyClasses.td}>
                Satisfied
              </td>
            </tr>
            <tr className={typographyClasses.tr}>
              <td className={typographyClasses.td}>
                Full
              </td>
              <td className={typographyClasses.td}>
                Ecstatic
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <p className={typographyClasses.p}>
        The king, seeing how much happier his subjects were, realized the error
        of his ways and repealed the joke tax. Jokester was declared a hero, and
        the kingdom lived happily ever after.
      </p>
      <p className={typographyClasses.p}>
        The moral of the story is: never underestimate the power of a good laugh
        and always be careful of bad ideas.
      </p>
    </div>
  );
}
